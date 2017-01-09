import serial
import json
import DeviceClient
import time

#Azure IoTHub Config###########
miiothub = ''
mikeyvalue = ''
midevice = ''
###############################

#generate JSON document from serial message
def generateJSON(serialmsg, student):
    messagetolist = (serialmsg.split(','))
    messagetolist.append(student)

    attributes = ["Humidity", "Temperature", "Student"]
    mydict = dict(zip(attributes, messagetolist))
    return (json.dumps((mydict)))

#configure serail Port
def configSerialPort():

    ser = serial.Serial() 
    ser.baudrate = 9600
    ser.port = 'COM6'
    return ser

#Open Serial Port
serial = configSerialPort()
serial.open()

#Create IoT Device
device = DeviceClient.DeviceClient(miiothub, midevice, mikeyvalue)
device.create_sas(600)

while True:
    serialmessage = str(serial.readline().decode('UTF-8').replace("\r\n",""))

    jsonmessage = generateJSON(serialmessage, "StudentXY")
    device.send(jsonmessage.encode('UTF-8'))
    
    print("Message sent: " + jsonmessage)
    
serial.close()  