import socket
import time
import numpy as np
import cv2

from lib.detect_face_features import *

def normalize(size, maxSize):
    return size / maxSize

def run():
    Test()
    
    CV_CAP_PROP_FRAME_WIDTH = 3
    CV_CAP_PROP_FRAME_HEIGHT = 4

    UDP_IP = "127.0.0.1"
    UDP_PORT = 6000
    UDP_TARGET_PORT = 6100
    
    MESSAGE = ""
    cap = cv2.VideoCapture(0)

    width = cap.get(CV_CAP_PROP_FRAME_WIDTH)
    height = cap.get(CV_CAP_PROP_FRAME_HEIGHT)

    trackFacialFeature = False
    
    mouthHeight = 0
    mouthLow = 0

    print("[ Webcam ] ")
    print("Width : ", width)
    print("Height : ", height)

    face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_alt.xml')

    print("IP : ", UDP_IP)
    print("Listen at  :", UDP_PORT)

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    
    listener = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    listener.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)

    listener.bind((UDP_IP, UDP_PORT))
    listener.setblocking(0)

    running = True
    print("Running....")

    while True:
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

        # Capture frame-by-frame
        ret, frame = cap.read()

        # Convert frame to gray
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        if trackFacialFeature:
            # Detect face features
            face_features = detect_facial_feature(gray)

            if len(face_features) > 0:
                mouthHeight = face_features[62][1]
                mouthLow = face_features[66][1]
        else:
            mouthHeight = 0
            mouthLow = 0
        
        # Detect faces        
        gray = cv2.equalizeHist(gray)
        faces = face_cascade.detectMultiScale(gray)

        # Draw rectangle around the faces
        #for (x, y, w, h) in faces:
            #cv2.rectangle(frame, (x, y), (x+w, y+h), (255, 0, 0), 2)

        # Display the frame
        #cv2.imshow('Press q to quit', frame)

        # Send info to game via UDP
        for (x, y, w, h) in faces:
            normX = normalize(x, width)
            normY = normalize(y, height)
            normWidth = normalize(w, width)
            normHeight = normalize(h, height)
            
            MESSAGE = str(normX) + "," + str(normY) + "," + str(normWidth) + "," + str(normHeight) + "," + str(mouthHeight) + "," + str(mouthLow) + "\n"
            sock.sendto(MESSAGE, (UDP_IP, UDP_TARGET_PORT))

        try:
            #Attempt to receive up to 1024 bytes of data
            (data, addr) = listener.recvfrom(1024) 
            #(data, addr) = listener.recvfrom(128 * 1024)
            print("Receive : ", data)
            
            if data == "U":
                trackFacialFeature = True
            elif data == "D":
                trackFacialFeature = False

            listener.sendto("Enable track feature : " + str(trackFacialFeature), (UDP_IP, UDP_TARGET_PORT))

        except socket.error:
            pass
        

    cap.release()
    cv2.destroyAllWindows()

    print("Stop...")

if __name__ == "__main__":
    run()
