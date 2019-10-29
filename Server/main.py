import socket
import time
import numpy as np
import cv2

def normalize(size, maxSize):
    return size / maxSize

def run():
    CV_CAP_PROP_FRAME_WIDTH = 3
    CV_CAP_PROP_FRAME_HEIGHT = 4

    UDP_IP = "127.0.0.1"
    UDP_PORT = 6100
    MESSAGE = ""

    cap = cv2.VideoCapture(0)

    width = cap.get(CV_CAP_PROP_FRAME_WIDTH)
    height = cap.get(CV_CAP_PROP_FRAME_HEIGHT)

    print("Width : " + str(width))
    print("Height : " + str(height))

    face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_alt.xml')

    print("UDP target IP:", UDP_IP)
    print("UDP target port:", UDP_PORT)

    running = True
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    print("Running....")

    while True:
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
        
        # Capture frame-by-frame
        ret, frame = cap.read()

        # Detect faces
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        gray = cv2.equalizeHist(gray)

        #faces = face_cascade.detectMultiScale(gray, 1.1, 4)
        faces = face_cascade.detectMultiScale(gray)

        # Draw rectangle around the faces
        for (x, y, w, h) in faces:
            cv2.rectangle(frame, (x, y), (x+w, y+h), (255, 0, 0), 2)
            
        # Display the frame
        cv2.imshow('Press q to quit', frame)

        # Send via UDP
        for (x, y, w, h) in faces:
            normX = normalize(x, width)
            normY = normalize(y, height)
            normWidth = normalize(w, width)
            normHeight = normalize(h, height)
            
            MESSAGE = str(normX) + "," + str(normY) + "," + str(normWidth) + "," + str(normHeight) + "\n"
            sock.sendto(MESSAGE, (UDP_IP, UDP_PORT))

    # Clear junk
    cap.release()
    cv2.destroyAllWindows()

    print("Stop...")


if __name__ == "__main__":
    run()

