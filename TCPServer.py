#using python 3.6
#Sources:
    #https://wiki.python.org/moin/TcpCommunication
    #https://docs.python.org/3/library/socket.html

import socket
import time

TCP_IP = '192.168.137.1'
TCP_PORT = 8080
BUFFER_SIZE = 1024
MAX_NO_OF_PLAYERS = 1

clientList= []

serverSocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)    #AF_INET = IPv4
serverSocket.bind((TCP_IP, TCP_PORT))
serverSocket.listen(2) #allow up to 2 unaccepted connections
print ("Server started and waiting for players")
    

while len(clientList) < MAX_NO_OF_PLAYERS:
    conn, addr = serverSocket.accept()
    print ('Connection address:', addr)
    clientList.append(conn)
    conn.send(str(len(clientList)).encode())

i = 0
while i < 10:
    conn.send(str(i).encode())
    time.sleep(1)
    print (i)
    i = i + 1


for client in clientList:
    client.send(str("Finished").encode())

    
while 1:
    for client in clientList:
        data = client.recv(BUFFER_SIZE)
        if data:
            print ("received data:", data.decode())
            #conn.send(data)  # echo back to client

conn.close()
