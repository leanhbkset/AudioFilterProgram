# import the necessary packages
import socket
import sys
import threading

import can
import time
from time import sleep
import os
import json


# --------------------------------------------------------------------- #
# Connect to Sensolution sensor via serial COM port                     #
# --------------------------------------------------------------------- #
class SocketListener(threading.Thread):
    # ----------------------------------------------------------------- #
    # Initializing vars                                                 #
    # ----------------------------------------------------------------- #
    def __init__(self, hostIP, hostPort):
        # Socket setting
        self.host = hostIP
        self.port = hostPort
        self.server_address = (self.host, self.port)
        self.socket_connected = False
        self.socket = self.init_socket()

    def init_socket(self):
        # Create a TCP/IP socket
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        return sock

    def socketconnect(self, ServerIP, Port):
        if (not self.socket_connected):
            try:
                print(sys.stderr, 'connecting to %s port %s' % (ServerIP, Port))
                self.server_address = (ServerIP, Port)
                self.socket.connect(self.server_address)
                print("Server connected")
                self.socket_connected = True
            except:
                pass
        else:
            print('Socket is disconnected.')

    def socketdisconnect(self):
        if self.socket_connected:
            try:
                self.socket.close()
                print('Socket is disconnected.')
                self.socket_connected = False
            except:
                pass
        else:
            print('socket is already disconnected')

    def sendmessage(self, messageRX):
        try:
            self.socket.sendall(messageRX)
        except:
            print("Server not connected")
            self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            self.socket_connected = False
            self.socketconnect(self.host, self.port)
            pass
            time.sleep(3)


# --------------------------------------------------------------------- #
# CAN connection                                                        #
# --------------------------------------------------------------------- #
class CANListener(threading.Thread):
    # ----------------------------------------------------------------- #
    # Initializing vars                                                 #
    # ----------------------------------------------------------------- #

    def __init__(self):
        threading.Thread.__init__(self)
        self.__flag = threading.Event()  # The flag used to pause the thread
        self.__flag.set()  # set to True
        self.__running = threading.Event()  # Used to stop the thread identification
        self.__running.set()  # Set running to True

        # CAN TX and RX parameters
        self.CANTX_isenable = False
        self.CANTX_baudrate = 100000
        self.CANTX_channel = 0
        self.CANTX_bus = []

        self.CANRX_isenable = False
        self.CANRX_baudrate = 100000
        self.CANRX_channel = 1
        self.CANRX_bus = []

        self.CANRX_host = '127.0.0.1'
        self.CANRX_port = 35363
        self.CANRX_socket = SocketListener(self.CANRX_host, self.CANRX_port)
        self.pause_cond = threading.Condition(threading.Lock())
        self.CANRX_threadstart = False

    def run(self):
        print('Start RX streaming...')
        while self.__running.isSet():
            self.__flag.wait()  # return immediately when it is True, block until the internal flag is True when it is False
            # Start streaming
            try:
                # while True:
                print('Received RX message...')
                message = self.CANRX_bus.recv()  # Wait until a message is received.

                c = '{0:f} {1:x} {2:x} '.format(message.timestamp, message.arbitration_id, message.dlc)
                s = ''
                for i in range(message.dlc):
                    s += '{0:x} '.format(message.data[i])
                print(' {}'.format(c + s))

                msgBuffer = self.PiCanMsg2ByteMsg(message.arbitration_id, message.data, message.dlc)

                # Send RX message via TCP socket
                if self.CANRX_socket.socket_connected == True:
                    print('sending socket msg: ', msgBuffer)
                    self.CANRX_socket.sendmessage(msgBuffer)

            except Exception as e:
                print(e)
                pass
            finally:
                # Stop streaming
                sleep(1)

    # this is should make the calling thread wait if pause() is
    # called while the thread is 'doing the thing', until it is
    # finished 'doing the thing'
    def pause(self):
        self.__flag.clear()  # Set to False to block the thread

    # should just resume the thread
    def resume(self):
        self.__flag.set()  # Set to True, let the thread stop blocking

    def CANEnable(self, channel, baudrate):
        print('Set CAN channel %d up with baudrate %d.' % (channel, baudrate))

        if channel == 0:
            canBaudrateCmd = "sudo /sbin/ip link set can0 up type can bitrate " + str(baudrate)
        elif channel == 1:
            canBaudrateCmd = "sudo /sbin/ip link set can1 up type can bitrate " + str(baudrate)

        # Set the CAN channel up
        try:
            os.system(canBaudrateCmd)
            if channel == 0:
                if not self.CANTX_isenable:
                    self.CANTX_isenable = True
                    self.CANTX_channel = channel
                    self.CANTX_baudrate = baudrate
                    self.CANTX_bus = can.interface.Bus(channel='can0', bustype='socketcan')
            elif channel == 1:
                if not self.CANRX_isenable:
                    self.CANRX_isenable = True
                    self.CANRX_channel = channel
                    self.CANRX_baudrate = baudrate
                    self.CANRX_bus = can.interface.Bus(channel='can1', bustype='socketcan')

                # Start socket
                print('Retry connect socket...')

                # if CANStream.CANRX_socket.socket_connected:
                #    CANStream.CANRX_socket.socketdisconnect()
                # self.CANRX_socket.socketconnect(self.CANRX_host, self.CANRX_port)

                # Start thread for streaming RX from Pican
                if self.CANRX_threadstart == False:
                    self.start()
                    self.CANRX_threadstart = True
                else:
                    print('Resume CAN RX Streaming')
                    self.resume()
                # elif msgStat == 0:
                #     time.sleep(1)
                #     print('Pause CAN RX Streaming')
                #     CANStream.pause()
                #     CANStream.CANRX_socket.socket_connected = False
                #     # CANStream.join()

        except OSError:
            print('Cannot find PiCAN board.')
            return False
            # exit()

        return True

    def CANDisable(self, channel):
        print('Set CAN channel %d down.' % channel)

        if channel == 0:
            canShutdownCmd = "sudo /sbin/ip link set can0 down"
        elif channel == 1:
            canShutdownCmd = "sudo /sbin/ip link set can1 down"

        # Set the CAN channel up
        try:
            if channel == 0:
                if self.CANTX_isenable:
                    self.CANTX_isenable = False
                    self.CANTX_channel = channel
                    self.CANTX_baudrate = 0
                    self.CANTX_bus.shutdown()
            elif channel == 1:
                # if self.CANRX_socket.socket_connected:
                #    self.CANRX_socket.socketdisconnect()

                if self.CANRX_isenable:
                    print('canRX shutdown ...')
                    self.CANRX_isenable = False
                    self.CANRX_channel = channel
                    self.CANRX_baudrate = 0
                    self.CANRX_bus.shutdown()

                if self.CANRX_threadstart == True:
                    self.pause()

            os.system(canShutdownCmd)
        except OSError:
            print('Cannot find PiCAN board.')
            return False
            # exit()

        return True

    def setupCAN(self, msgBuffer):
        canChannel = msgBuffer[0]
        if canChannel == 0:
            # Setting for can0: TX
            self.CANTX_channel = canChannel
            canStatus = msgBuffer[1]
            print('CAN TX Setting: ', canStatus)
            if canStatus == 1:
                # Up CAN channel 0
                canBaudrate = msgBuffer[2]
                if canBaudrate == 0:
                    canBaudrateDec = 100000
                elif canBaudrate == 1:
                    canBaudrateDec = 125000
                elif canBaudrate == 2:
                    canBaudrateDec = 250000
                elif canBaudrate == 3:
                    canBaudrateDec = 500000
                self.CANEnable(canChannel, canBaudrateDec)
            elif canStatus == 0:
                # Shut down can channel 0
                self.CANDisable(canChannel)

        elif canChannel == 1:
            # Setting for can1: RX
            self.CANRX_channel = canChannel
            canStatus = msgBuffer[1]
            print('CAN TX Setting: ', canStatus)
            if canStatus == 1:
                # Up CAN channel 0
                canBaudrate = msgBuffer[2]
                if canBaudrate == 0:
                    canBaudrateDec = 100000
                elif canBaudrate == 1:
                    canBaudrateDec = 125000
                elif canBaudrate == 2:
                    canBaudrateDec = 250000
                elif canBaudrate == 3:
                    canBaudrateDec = 500000
                self.CANEnable(canChannel, canBaudrateDec)
            elif canStatus == 0:
                # Shut down can channel 0
                self.CANDisable(canChannel)

    def sendCANTXMsg(self, msgID, msgData, isExtended):
        # Send CAN TX message
        try:
            if (self.CANTX_isenable):
                if not isExtended:  # Standard CAN 2.0 - Non extend CAN message
                    msg = can.Message(arbitration_id=msgID, data=msgData, extended_id=isExtended)
                else:
                    msg = can.Message(arbitration_id=msgID, is_extended_id=True, data=msgData)
                self.CANTX_bus.send(msg)
                # bus0.flush_tx_buffer()
                # count += 1
                # print("Total count = ", count)
            else:
                print('CAN TX channel is not enable yet. Please up the CAN channel 0!')
                return False
        except OSError:
            print('Network is down! Please up the CAN channel 0')
            return False

        return True

    def ByteMsg2PiCanMsg(self, dataBuffer):
        msgHeader = dataBuffer[0]
        print('msgHeader: ', msgHeader)
        msgID = 0
        msgData = []
        msgLen = 0
        msgCRC = 0

        if msgHeader == 170:
            # Send CAN TX
            messageLength = dataBuff[1:3]
            msgLen = int.from_bytes(messageLength, 'big')

            messageID = dataBuff[3:7]
            data = dataBuff[7:msgLen + 3]

            msgID = int.from_bytes(messageID, 'big')
            msgData = data

            # Check sum
            msgCRC = 1

        return msgID, msgData, msgLen, msgCRC

    def PiCanMsg2ByteMsg(self, msgID, msgData, msgLen):
        msgHeader = 170
        msgHeader_byte = msgHeader.to_bytes(1, 'big')
        msgLength = msgLen + 4
        msgLen_byte = msgLength.to_bytes(2, 'big')

        msgID_byte = msgID.to_bytes(4, 'big')

        msgData_byte = msgData

        msgChecksum = msgHeader_byte + msgLen_byte + msgID_byte + msgData_byte
        msgChecksum_byte = self.calculate_checksum(msgChecksum)

        print('msgChecksum: ', msgChecksum)
        print('checksumbyte: ', msgChecksum_byte)

        dataBuffer = msgHeader_byte + msgLen_byte + msgID_byte + msgData_byte + msgChecksum_byte

        return dataBuffer

    # Checksum
    def calculate_checksum(self, message):
        r = 0
        for c in message:
            r ^= c
        return bytes([r])


# --------------------------------------------------------------------- #
# Main custom functions                                                 #
# --------------------------------------------------------------------- #
if __name__ == '__main__':
    # Initializing for processing input data
    CANStream = CANListener()

    while True:
        inputdata = input()

        if inputdata:
            print(inputdata)
            print("CAN MESSAGE")

            if inputdata[0:7] == "::ffff:":
                Host = inputdata[7:]
                print('Current IP: ', CANStream.CANRX_host)
                print('New IP: ', Host)
                if Host != CANStream.CANRX_host:
                    CANStream.CANRX_host = Host
                    CANStream.CANRX_port = 35363
                    print('RX Server IP: ', Host)
                    # if CANStream.CANRX_socket.socket_connected:
                    #    CANStream.CANRX_socket.socketdisconnect()
                    CANStream.CANRX_socket.socketconnect(CANStream.CANRX_host, CANStream.CANRX_port)
            else:
                datajson = json.loads(inputdata)
                # print(datajson)
                dataBuff = datajson["data"]
                print(dataBuff)

                msgHeader = dataBuff[0]
                print('msgHeader: ', msgHeader)

                if msgHeader == 170:
                    # Send CAN TX
                    if CANStream.CANTX_isenable == True:
                        msgID, msgData, msgLen, msgCRC = CANStream.ByteMsg2PiCanMsg(dataBuff)
                        if msgID < 65535:  # Non extended CAN Message
                            CANStream.sendCANTXMsg(msgID, msgData, isExtended=False)
                        else:
                            CANStream.sendCANTXMsg(msgID, msgData, isExtended=True)
                        print("CAN TX msgID: %d, CAN TX Len: %d" % (msgID, msgLen))
                        print("CAN TX Data: ", msgData)
                    else:
                        print("CAN TX is not enable yet.")

                elif msgHeader == 0:
                    # Setting for can0: TX
                    CANStream.setupCAN(dataBuff)
                elif msgHeader == 1:
                    # Setting socket param
                    # CANStream.CANRX_host = '127.0.0.1'
                    # CANStream.CANRX_port = 35363

                    # Setting for can1: RX
                    CANStream.setupCAN(dataBuff)
                    time.sleep(1)

                else:
                    print("No cmd!")

        else:
            print("There is no data!")
