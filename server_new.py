import socket
from mss.windows import MSS as mss
import cv2
import time
import  numpy as np
import json
import pyautogui
from threading import Thread
from ctypes import *
import ctypes
from ctypes.wintypes import (HMONITOR, BOOL,HDC ,LPARAM)
import win32api


# a = win32api.EnumDisplayDevices(None,0)
lst  = win32api.EnumDisplayMonitors()
lst = sorted(lst , key= lambda l : l[2][0])
print(lst)
# print(mon.rcMonitor)
def start_server():
    # host = '192.168.0.107'
    hostname = socket.gethostname()
    host = socket.gethostbyname(hostname)
    # host = "192.168.0.107"
    # host = '10.42.10.107'
    port = 9999

    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind((host, port))


    print(f"Server listening on {host}:{port}")
    try:
        server_socket.listen(1)
        while True:
            client_socket, addr = server_socket.accept()
            print(f"Connection from {addr}")


            thread = Thread(target=send_screen_stream, args=[client_socket])
            thread.start()
            # Sending screen stream for monitor 2
            # send_screen_stream(client_socket, monitor=3)
    except Exception as e:
        print(f"Error: {e}")

    finally:
        client_socket.close()


def send_screen_stream(client_socket):
    screens = 3
    WIDTH = 1920
    HEIGHT = 1080
    with mss() as sct:
        # Get the coordinates of the specifi    ed monitor


        while True:
            # RECEIVING PART -------------------------------------------------------------
            # json_data = client_socket.recv(1024).decode('utf-8')

            # Load JSON into a dictionary
            # info_dictionary = json.loads(json_data)

            # Process the received information
            # process_info(info_dictionary)

            mouse_pos = pyautogui.position()

            #SENDING PART----------------------------------------------------------------------
            # Capture the screen of the specified monitor
            og = b''
            for i in range(screens):

                mon = sct.monitors[i+1]
                left = sct.monitors[i+1]["left"]
                print(left)

                mon["width"] = WIDTH
                mon["height"] = HEIGHT
                mon["left"] = WIDTH * i

                # screenshot = sct.grab(mon)
                screenshot = np.array(sct.grab(mon))
                # print(mouse_pos)
                cursor = ctypes.wintypes.POINT()
                user32 = ctypes.windll.user32
                # user32.SetProcessDPIAware(2)
                user32.GetCursorPos(ctypes.byref(cursor))
                res= [user32.GetSystemMetrics(1), user32.GetSystemMetrics(2)]
                # print(cursor.x , cursor.y)
                print(res)
                mouse_pos = cursor

                print(mouse_pos.x)
                if((mouse_pos.x < lst[i][2][2])and(mouse_pos.x>lst[i][2][0])):
                    cv2.circle(screenshot,(mouse_pos.x -lst[i][2][0] ,mouse_pos.y- (lst[i][2][1])), 10 , (250,0,250),-1)
                print("shot taken")
                success, encoded_image = cv2.imencode('.png', screenshot)
                image_bytes = encoded_image.tobytes()
                size = len(image_bytes)
                client_socket.sendall(size.to_bytes(4, byteorder='little'))

                og = b''.join([og, image_bytes])
                # time.sleep(0.1)
                # Send the size of the image first

                print("byte sent: size - " + str(size))

            # Convert the image to bytes

            # Send image data in chunks
            chunk_size = 1024
            # print(og[0])

            for j in range(0, len(og), chunk_size):
                client_socket.sendall(og[j:j + chunk_size])

            # client_socket.sendall(image_bytes)
            # Add a small delay to control the streaming speed

def process_info(info_dictionary):
    # Process the received information
    print("Received information:")
    for key, value in info_dictionary.items():
        print(f"{key}: {value}")
if __name__ == "__main__":
    start_server()
