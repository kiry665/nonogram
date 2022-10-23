from dataclasses import replace
from re import I
import cv2

for j in range(9,21):
    s = str(j)
    img = cv2.imread(s+'.jpg', cv2.IMREAD_GRAYSCALE)

    row = img.shape[0]
    col = img.shape[1]

    print('----')

    #создание пустого массива
    patern = [0]*row
    for i in range(row):
        patern[i] = [0]*col

    for i in range(row):
        for j in range(col):
            if (img[i,j] == 0):
                patern[i][j] = 1
            elif (img[i,j] == 255):
                patern[i][j] = 0

    with open(r""+s+".txt", "w") as file:
        #ile.write(str(row) + " " + str(col) + "\n")
        for i in range(row):
            s = str(patern[i])
            s = s.replace('[', '')
            s = s.replace(']', '')
            s = s.replace(' ', '')
            print(s)
            file.write(s + ";")
        

