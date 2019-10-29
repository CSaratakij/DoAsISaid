import socket
import time
import numpy as np
import cv2

UDP_IP = "127.0.0.1"
UDP_PORT = 6100
MESSAGE = ""

cap = cv2.VideoCapture(0)

face_cascade = cv2.CascadeClassifier('haarcascade_frontalface_alt.xml')
eyes_cascade = cv2.CascadeClassifier('haarcascade_eye_tree_eyeglasses.xml')

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
        
         #-- In each face, detect eyes
        faceROI = gray[y:y+h,x:x+w]
        eyes = eyes_cascade.detectMultiScale(faceROI)
        
        for (x2,y2,w2,h2) in eyes:
            eye_center = (x + x2 + w2//2, y + y2 + h2//2)
            radius = int(round((w2 + h2)*0.25))
            frame = cv2.circle(frame, eye_center, radius, (255, 255, 0 ), 4)
    
    # Display the frame
    cv2.imshow('Press q to quit', frame)

    for (x, y, w, h) in faces:
        MESSAGE = str(x) + "," + str(y) + "," + str(w) + "," + str(h)
        sock.sendto(MESSAGE, (UDP_IP, UDP_PORT))

# When everything done, release the capture
cap.release()
cv2.destroyAllWindows()

print("Stop...")
