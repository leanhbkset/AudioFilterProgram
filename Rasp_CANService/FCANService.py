# import the necessary packages
import socket
import select
import sys
import threading
import logging
import datetime

# import can
import time
from time import sleep
import os
import json
import uuid

# --------------------------------------------------------------------- #
# DEFINE CONSTANT                                                       #
# --------------------------------------------------------------------- #
SERVER_PORT = 3001
# CLIENT_PORT = SERVER_PORT + 1000
CLIENT_PORT = 35363
RECV_BUFFER = 4096

CAN_BAUDRATE_0 = 100000
CAN_BAUDRATE_1 = 125000
CAN_BAUDRATE_2 = 250000
CAN_BAUDRATE_3 = 500000

# CanTX - CanRX setting  status
CANTX_CONNECT_SUCCESS = b'TXCONNECTSUCCESS'
CANTX_CONNECT_FAIL = b'TXCONNECT_FAILED'
CANTX_DISCONNECT_SUCCESS = b'TX__DISCONNECTED'
CANTX_DISCONNECT_FAIL = b'TXDISCONNECTFAIL'
CANTX_CONNECTION_BUSY = b'TX__CONNECT_BUSY'
CANTX_SENDMSG_SUCCESS = b'TXSENDMSGSUCCESS'
CANTX_SENDMSG_FAIL = b'TXSENDMSG_FAILED'

CANRX_CONNECT_SUCCESS = b'RXCONNECTSUCCESS'
CANRX_CONNECT_FAIL = b'RXCONNECT_FAILED'
CANRX_DISCONNECT_SUCCESS = b'RX__DISCONNECTED'
CANRX_DISCONNECT_FAIL = b'RXDISCONNECTFAIL'

# Msg Checksum error
MSG_ERROR = b'CHECKSUM_ERROR'
MSG_RESPONSE_HEADER = b'\x55'

# Initialize logger
# set up logging to file - see previous section for more details
logging.basicConfig(level=logging.DEBUG,
                    format='%(asctime)s %(name)-12s %(levelname)-8s %(message)s',
                    datefmt='%m-%d %H:%M',
                    filename='FCANService.log',
                    filemode='w')
# define a Handler which writes INFO messages or higher to the sys.stderr
console = logging.StreamHandler()
console.setLevel(logging.INFO)
console.setLevel(logging.DEBUG)
# set a format which is simpler for console use
formatter = logging.Formatter('%(name)-12s: %(levelname)-8s %(message)s')
# tell the handler to use this format
console.setFormatter(formatter)
# add the handler to the root logger
logging.getLogger('').addHandler(console)

# Now, we can log to the root logger, or any other logger. First the root...
logging.debug('Testing logging')


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
        # self.socket = self.init_socket()

    def init_socket(self):
        # Create a TCP/IP socket
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

        return sock

    def socketconnect(self, ServerIP, Port):
        if (not self.socket_connected):
            try:
                logging.info("_FUNC_socketconnect_RX Server Connecting to %s port %s" % (ServerIP, Port))
                self.socket = self.init_socket()
                self.server_address = (ServerIP, Port)
                self.socket.connect(self.server_address)
                logging.info("_FUNC_socketconnect_RX Server connected")
                self.socket_connected = True
            except:
                logging.info("_FUNC_socketconnect_RX Socket connect failed.")
                pass
        else:
            logging.info("_FUNC_socketconnect_RX Socket is disconnected.")

    def socketdisconnect(self):
        if self.socket_connected:
            try:
                self.socket.close()
                logging.info("_FUNC_socketdisconnect_RX Socket is disconnected.")
                self.socket_connected = False
            except:
                pass
        else:
            logging.info("_FUNC_socketdisconnect_RX Socket is already disconnected")

    def sendmessage(self, messageRX):
        try:
            self.socket.sendall(messageRX)
        except:
            logging.info("_FUNC_sendmessage_Server not connected")
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

        # Device ID ~ Mac Address
        self.deviceID = uuid.getnode().to_bytes(6, 'big')

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
        self.CANRX_port = CLIENT_PORT
        self.CANRX_socket = SocketListener(self.CANRX_host, self.CANRX_port)
        self.pause_cond = threading.Condition(threading.Lock())
        self.CANRX_threadstart = False

    def run(self):
        logging.info("_FUNC_run_Start RX streaming...")
        while self.__running.isSet():
            self.__flag.wait()  # return immediately when it is True, block until the internal flag is True when it is False
            # Start streaming
            try:
                # while True:
                logging.info("_FUNC_run_Received RX message...")
                message = self.CANRX_bus.recv()  # Wait until a message is received.

                c = '{0:f} {1:x} {2:x} '.format(message.timestamp, message.arbitration_id, message.dlc)
                s = ''
                for i in range(message.dlc):
                    s += '{0:x} '.format(message.data[i])
                logging.info(' {}'.format(c + s))

                msgBuffer = self.PiCanMsg2ByteMsg(message.arbitration_id, message.data, message.dlc)
                # msgBuffer = b'\xaa\x00\x00\x11\x22\x33\x44\x82'   for debugging
                # Send RX message via TCP socket
                if self.CANRX_socket.socket_connected == True:
                    logging.info("_FUNC_run_Forwarding RX socket msg: ", msgBuffer)
                    self.CANRX_socket.sendmessage(msgBuffer)

            except Exception as e:
                logging.error(e)
                pass
            finally:
                # Stop streaming
                sleep(1)

    # This is should make the calling thread wait if pause() is
    # called while the thread is 'doing the thing', until it is
    # finished 'doing the thing'
    def pause(self):
        self.__flag.clear()  # Set to False to block the thread

    # Should just resume the thread
    def resume(self):
        self.__flag.set()  # Set to True, let the thread stop blocking

    def CANEnable(self, channel, baudrate):
        logging.info("_FUNC_CANEnable_Set CAN channel {0} up with baudrate {1}".format(channel, baudrate))

        if channel == 0:
            canBaudrateCmd = "sudo /sbin/ip link set can0 up type can bitrate " + str(baudrate)
        elif channel == 1:
            canBaudrateCmd = "sudo /sbin/ip link set can1 up type can bitrate " + str(baudrate)

        # Set the CAN channel up
        try:

            if channel == 0:
                if not self.CANTX_isenable:
                    os.system(canBaudrateCmd)
                    self.CANTX_isenable = True
                    self.CANTX_channel = channel
                    self.CANTX_baudrate = baudrate
                    # self.CANTX_bus = can.interface.Bus(channel='can0', bustype='socketcan')
                else:
                    logging.info("_FUNC_CANEnable_CANTX is already up")
                    return True
            elif channel == 1:
                if not self.CANRX_isenable:
                    os.system(canBaudrateCmd)
                    self.CANRX_isenable = True
                    self.CANRX_channel = channel
                    self.CANRX_baudrate = baudrate
                    # self.CANRX_bus = can.interface.Bus(channel='can1', bustype='socketcan')
                else:
                    logging.info("_FUNC_CANEnable_CANRX is already up")
                    return True

                # Start socket
                print('Retry connect socket...')

                if CANStream.CANRX_socket.socket_connected:
                    logging.info("_FUNC_CANEnable_CANRX socket OFF")
                    CANStream.CANRX_socket.socketdisconnect()
                    sleep(1)

                self.CANRX_socket.socketconnect(self.CANRX_host, self.CANRX_port)

                # Start thread for streaming RX from Pican
                if self.CANRX_threadstart == False:
                    self.start()
                    self.CANRX_threadstart = True
                else:
                    logging.info("_FUNC_CANEnable_Resume CAN RX Streaming")
                    self.resume()

        except OSError:
            logging.error("_FUNC_CANEnable_Cannot find PiCAN board.")
            return False

        return True

    def CANDisable(self, channel):
        logging.info("_FUNC_CANDisable_Set CAN channel {} down".format(channel))

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
                if self.CANRX_threadstart == True:
                    logging.debug("_FUNC_CANDisable_CAN RX Thread pause.")
                    self.pause()

                if self.CANRX_isenable:
                    logging.info("_FUNC_CANDisable_canRX shutdown ...")
                    self.CANRX_isenable = False
                    self.CANRX_channel = channel
                    self.CANRX_baudrate = 0
                    self.CANRX_bus.shutdown()

            os.system(canShutdownCmd)
        except OSError:
            logging.error('_FUNC_CANDisable_Cannot find PiCAN board.')
            return False

        return True

    def setupCAN(self, msgBuffer):
        canSetupStatus = -10  # Setup CAN status

        canChannel = msgBuffer[1]
        if canChannel == 0:
            # Setting for can0: TX
            self.CANTX_channel = canChannel
            canStatus = msgBuffer[2]

            if canStatus == 1:
                # Up CAN channel 0
                canBaudrate = msgBuffer[3]
                if canBaudrate == 0:
                    canBaudrateDec = CAN_BAUDRATE_0
                elif canBaudrate == 1:
                    canBaudrateDec = CAN_BAUDRATE_1
                elif canBaudrate == 2:
                    canBaudrateDec = CAN_BAUDRATE_2
                elif canBaudrate == 3:
                    canBaudrateDec = CAN_BAUDRATE_3

                TXstatus = self.CANEnable(canChannel, canBaudrateDec)
                if TXstatus:
                    # CanTX connect succeed
                    canSetupStatus = CANTX_CONNECT_SUCCESS
                else:
                    # CanTX connect failed
                    canSetupStatus = CANTX_CONNECT_FAIL
            elif canStatus == 0:
                # Shut down can channel 0
                TXstatus = self.CANDisable(canChannel)
                if TXstatus:
                    # CanTX disconnect succeed
                    canSetupStatus = CANTX_DISCONNECT_SUCCESS
                else:
                    # CanTX disconnect failed
                    canSetupStatus = CANTX_DISCONNECT_FAIL

        elif canChannel == 1:
            # Setting for can1: RX
            self.CANRX_channel = canChannel
            canStatus = msgBuffer[2]

            if canStatus == 1:
                # Up CAN channel 0
                canBaudrate = msgBuffer[3]
                if canBaudrate == 0:
                    canBaudrateDec = CAN_BAUDRATE_0
                elif canBaudrate == 1:
                    canBaudrateDec = CAN_BAUDRATE_1
                elif canBaudrate == 2:
                    canBaudrateDec = CAN_BAUDRATE_2
                elif canBaudrate == 3:
                    canBaudrateDec = CAN_BAUDRATE_3

                RXstatus = self.CANEnable(canChannel, canBaudrateDec)
                if RXstatus:
                    # CanRX connect succeed
                    canSetupStatus = CANRX_CONNECT_SUCCESS
                else:
                    # CanRX connect failed
                    canSetupStatus = CANRX_CONNECT_FAIL
            elif canStatus == 0:
                # Shut down can channel 0
                RXstatus = self.CANDisable(canChannel)
                if RXstatus:
                    # CanRX disconnect succeed
                    canSetupStatus = CANRX_DISCONNECT_SUCCESS
                else:
                    # CanRX disconnect failed
                    canSetupStatus = CANRX_DISCONNECT_FAIL

        return canSetupStatus

    def sendCANTXMsg(self, msgID, msgData, isExtended):
        # Send CAN TX message
        try:
            logging.info("_FUNC_sendCANTXMsg_Send TX CAN status: ", self.CANTX_isenable)
            if (self.CANTX_isenable):
                if not isExtended:  # Standard CAN 2.0 - Non extend CAN message
                    logging.info("_FUNC_sendCANTXMsg_CAN Msg: {0} {1}".format(msgID, msgData))
                    # msg = can.Message(arbitration_id=msgID, data=msgData, extended_id=False)

                else:
                    logging.info("_FUNC_sendCANTXMsg_CAN Msg: {0} {1}".format(msgID, msgData))
                    # msg = can.Message(arbitration_id=msgID, is_extended_id=True, data=msgData)

                logging.info("_FUNC_sendCANTXMsg_CANTX sending msg...")
                # self.CANTX_bus.send(msg)
                # bus0.flush_tx_buffer()
            else:
                logging.error('_FUNC_sendCANTXMsg_CAN TX channel is not enable yet. Please up the CAN channel 0!')
                return False
        except OSError:
            logging.error('_FUNC_sendCANTXMsg_Network is down! Please up the CAN channel 0')
            return False

        return True

    def ByteMsg2PiCanMsg(self, dataBuffer):
        logging.info("_FUNC_ByteMsg2PiCanMsg_Databuffer: {}".format(dataBuffer))

        msgHeader = dataBuffer[0]
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
            msgCRC = 1  # dataBuffer[-1]

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

        dataBuffer = msgHeader_byte + msgLen_byte + msgID_byte + msgData_byte + msgChecksum_byte

        logging.info("_FUNC_PiCanMsg2ByteMsg_Databuffer output: {}".format(dataBuffer))

        return dataBuffer

    # Response message
    def createResponseMessage(self, msgCode):
        msgHeader = MSG_RESPONSE_HEADER
        msgDeviceID = self.deviceID

        msgChecksum = msgHeader + msgDeviceID + msgCode
        msgChecksum_byte = self.calculate_checksum(msgChecksum)

        dataBuffer = msgChecksum + msgChecksum_byte

        logging.info("_FUNC_createResponseMessage_Databuffer output: {}".format(dataBuffer))

        return dataBuffer

    # Response message
    def createInitMessage(self):
        msgHeader = MSG_RESPONSE_HEADER
        msgDeviceID = self.deviceID

        # CAN Status
        if self.CANTX_isenable:
            msgCANTX = b'\x01'
        else:
            msgCANTX = b'\x00'

        if self.CANRX_isenable:
            msgCANRX = b'\x01'
        else:
            msgCANRX = b'\x00'

        # No of CAN channels
        msgCANNo = b'\x10'

        # CAN TX Message
        msgChecksum = msgHeader + msgDeviceID + msgCANTX + msgCANRX + msgCANNo
        msgChecksum_byte = self.calculate_checksum(msgChecksum)
        dataBuffer = msgChecksum + msgChecksum_byte

        logging.info("_FUNC_createInitMessage_Databuffer output: {}".format(dataBuffer))

        return dataBuffer

    # Checksum
    def calculate_checksum(self, message):
        r = 0
        for c in message:
            r ^= c
        return bytes([r])


# --------------------------------------------------------------------- #
# Custom Function                                                       #
# --------------------------------------------------------------------- #
def broadcast_data(sock, message):
    # Do not send the message to master socket and the client who has send us the message
    for socket in CONNECTION_LIST:
        if socket != server_socket and socket != sock:
            try:
                socket.sendall(message)
            except:
                # broken socket connection may be, chat client pressed ctrl+c for example
                socket.close()
                CONNECTION_LIST.remove(socket)


# --------------------------------------------------------------------- #
# Main custom functions                                                 #
# --------------------------------------------------------------------- #
if __name__ == '__main__':
    # Initializing CAN Channel for TX/RX
    CANStream = CANListener()

    # Setting Server Socket
    # List to keep track of socket descriptors
    CONNECTION_LIST = []

    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    # this has no effect, why ?
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server_socket.bind(("0.0.0.0", SERVER_PORT))
    server_socket.listen(10)

    # Add server socket to the list of readable connections
    CONNECTION_LIST.append(server_socket)

    logging.info("_FUNC_main_Databuffer output: {}".format(SERVER_PORT))

    while True:
        # Get the list sockets which are ready to be read through select
        read_sockets, write_sockets, error_sockets = select.select(CONNECTION_LIST, [], [])

        for sock in read_sockets:
            # New connection
            if sock == server_socket:
                # Handle the case in which there is a new connection recieved through server_socket
                sockfd, addr = server_socket.accept()
                CONNECTION_LIST.append(sockfd)

                logging.info("_FUNC_main_Client ({0}, {1}) is offline".format(addr[0], addr[1]))

                broadcast_data(sockfd, "[%s:%s] entered room\n" % addr)

                Host = addr[0]
                logging.info("_FUNC_main_IP: {}".format(addr[0]))
                logging.info("_FUNC_main_Current IP: {}".format(CANStream.CANRX_host))
                logging.info("_FUNC_main_New IP: {}".format(Host))
                if CANStream.CANRX_socket.socket_connected:
                    if Host != CANStream.CANRX_host:
                        CANStream.CANRX_host = Host
                        CANStream.CANRX_port = CLIENT_PORT
                        logging.info("_FUNC_main_Connecting RX Server IP: {}".format(Host))
                    CANStream.CANRX_socket.socketdisconnect()
                    sleep(1)
                    CANStream.CANRX_socket.socketconnect(Host, CANStream.CANRX_port)
                else:
                    if Host != CANStream.CANRX_host:
                        CANStream.CANRX_host = Host
                        CANStream.CANRX_port = CLIENT_PORT
                        logging.info("_FUNC_main_Connecting RX Server IP: {}".format(Host))

                    CANStream.CANRX_socket.socketconnect(CANStream.CANRX_host, CANStream.CANRX_port)

                # Update the device information to Server & Number CAN Channel.
                # Send initial message to server for Device Info
                sock.sendall(CANStream.createInitMessage())

            # Some incoming message from a client
            else:
                # Data recieved from client, process it
                try:
                    # In Windows, sometimes when a TCP program closes abruptly,
                    # a "Connection reset by peer" exception will be thrown
                    inputdata = sock.recv(RECV_BUFFER)

                    if inputdata:
                        logging.info("_FUNC_main_Receive msg from Client: {}".format(inputdata))

                        try:
                            dataBuff = inputdata

                            msgHeader = dataBuff[0]

                            # Check message checksum
                            MsgCRC = dataBuff[-1]
                            MsgData = dataBuff[:-1]
                            Checksum = CANStream.calculate_checksum(MsgData)
                            Msg_checksum = int.from_bytes(Checksum, 'big')

                            print('Msg checksum', Msg_checksum)
                            print('MsgCRC', MsgCRC)
                            CANResponseStatus = b''

                            if Msg_checksum == MsgCRC:
                                # if True:
                                if msgHeader == 170:
                                    # Send CAN TX
                                    logging.info("_FUNC_main_CANTX Message: {}".format(inputdata))
                                    logging.info("_FUNC_main_CANTX Status: {}".format(CANStream.CANTX_isenable))
                                    if CANStream.CANTX_isenable == True:
                                        msgID, msgData, msgLen, msgCRC = CANStream.ByteMsg2PiCanMsg(dataBuff)
                                        CANResponseStatus = False
                                        if msgID < 65535:  # Non extended CAN Message
                                            CANTXsendMsgStatus = CANStream.sendCANTXMsg(msgID, msgData,
                                                                                        isExtended=False)
                                            logging.info("_FUNC_main_Sent CANTX done")
                                        else:
                                            CANTXsendMsgStatus = CANStream.sendCANTXMsg(msgID, msgData, isExtended=True)

                                        if CANTXsendMsgStatus:
                                            CANResponseStatus = CANTX_SENDMSG_SUCCESS
                                        else:
                                            CANResponseStatus = CANTX_SENDMSG_FAIL
                                    else:
                                        logging.info("_FUNC_main_CAN TX is not enable yet.")

                                elif msgHeader == 85:
                                    msgCANConfig = dataBuff[1]
                                    if msgCANConfig == 0:
                                        # Setting for can0: TX
                                        logging.info("_FUNC_main_CANTX Config: {}".format(dataBuff))
                                        CANResponseStatus = CANStream.setupCAN(dataBuff)
                                    elif msgCANConfig == 1:
                                        # Setting socket param
                                        # Setting for can1: RX
                                        logging.info("_FUNC_main_CANRX Config: {}".format(dataBuff))
                                        CANResponseStatus = CANStream.setupCAN(dataBuff)
                                        time.sleep(1)

                                    # Check socket connection
                                    elif msgCANConfig == 11:
                                        CANStream.CANRX_socket.socketdisconnect()
                                    elif msgCANConfig == 10:
                                        CANStream.CANRX_socket.socketconnect(CANStream.CANRX_host, CANStream.CANRX_port)
                                else:
                                    logging.debug("_FUNC_main_Cmd format is not match.")
                            else:
                                # Checksum error
                                CANResponseStatus = MSG_ERROR

                            CANResponseMsg = CANStream.createResponseMessage(CANResponseStatus)
                            sock.sendall(CANResponseMsg)
                        except:
                            logging.debug("_FUNC_main_Message is not correct format")
                            continue

                    else:
                        # print("There is no data!")
                        continue
                except:
                    # broadcast_data(sock, "Client (%s, %s) is offline" % addr)
                    logging.info("Client ({0}, {1}) is offline".format(addr[0], addr[1]))
                    sock.close()
                    if sock in CONNECTION_LIST:
                        CONNECTION_LIST.remove(sock)
                    continue

    server_socket.close()
