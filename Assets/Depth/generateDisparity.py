import numpy as np 
import cv2
from matplotlib import pyplot as plt

 
baseline=100
focal_length= 4.77424789e+02

Q=np.float32([[ 1.00000000e+00,  0.00000000e+00,  0.00000000e+00,  6.51067677e+02],
 [ 0.00000000e+00,  1.00000000e+00,  0.00000000e+00,  4.89214800e+02],
 [ 0.00000000e+00,  0.00000000e+00,  0.00000000e+00,  1.19511214e+03],
 [ 0.00000000e+00,  0.00000000e+00, -4.65810195e-04,  0.00000000e+00]])


# Reading the mapping values for stereo image rectification
cv_file = cv2.FileStorage("improved_params3.xml", cv2.FILE_STORAGE_READ)
Left_Stereo_Map_x = cv_file.getNode("Left_Stereo_Map_x").mat()
Left_Stereo_Map_y = cv_file.getNode("Left_Stereo_Map_y").mat()
Right_Stereo_Map_x = cv_file.getNode("Right_Stereo_Map_x").mat()
Right_Stereo_Map_y = cv_file.getNode("Right_Stereo_Map_y").mat()
cv_file.release()
 
def write_ply(fn, verts, colors):
    verts = verts.reshape(-1, 3)
    colors = colors.reshape(-1, 3)
    verts = np.hstack([verts, colors])
    with open(fn, 'wb') as f:
        f.write((ply_header % dict(vert_num=len(verts))).encode('utf-8'))
        np.savetxt(f, verts, fmt='%f %f %f %d %d %d ')


while True:
  # Capturing and storing left and right camera images
    imgL= cv2.imread(r'C:\Users\Benjamin\Documents\ROV-VR-App\Assets\Depth\img1.jpg', cv2.IMREAD_GRAYSCALE)[200:850, 600:1400]
    imgR= cv2.imread(r'C:\Users\Benjamin\Documents\ROV-VR-App\Assets\Depth\img2.jpg', cv2.IMREAD_GRAYSCALE)[200:850, 600:1400]

    
    Left_nice= cv2.remap(imgL,
                Left_Stereo_Map_x,
                Left_Stereo_Map_y,
                cv2.INTER_LANCZOS4,
                cv2.BORDER_CONSTANT,
                0)
        
        # Applying stereo image rectification on the right image
    Right_nice= cv2.remap(imgR,
                Right_Stereo_Map_x,
                Right_Stereo_Map_y,
                cv2.INTER_LANCZOS4,
                cv2.BORDER_CONSTANT,
                0)
        

    # creates StereoBm object 
    stereo = cv2.StereoSGBM_create(numDisparities =160, blockSize =1)
    
    # computes disparity
    disparity = stereo.compute(Left_nice, Right_nice)#.astype(np.float32) / 16.0

    distance = (baseline * focal_length) / disparity

    dispL=disparity
    stereoR=cv2.ximgproc.createRightMatcher(stereo)

    dispR= stereoR.compute(Right_nice,Left_nice)
    dispL= np.int16(dispL)
    dispR= np.int16(dispR)

        # Using the WLS filter
    lmbda = 80000
    sigma = 1.8
    visual_multiplier = 1.0
    wls_filter = cv2.ximgproc.createDisparityWLSFilter(matcher_left=stereo)
    wls_filter.setLambda(lmbda)
    wls_filter.setSigmaColor(sigma)


    filteredImg= wls_filter.filter(dispL,Left_nice,None,dispR)
    filteredImg = cv2.normalize(src=filteredImg, dst=filteredImg, beta=0, alpha=255, norm_type=cv2.NORM_MINMAX);
    filteredImg = np.uint8(filteredImg)
    #cv2.imshow('Disparity Map', filteredImg)
    disp= ((disparity.astype(np.float32)/ 16)-2)/128 # Calculation allowing us to have 0 for the most distant object able to detect

    ##    # Resize the image for faster executions
    #dispR= cv2.resize(disp,None,fx=0.7, fy=0.7, interpolation = cv2.INTER_AREA)

        # Filtering the Results with a closing filter
    kernel= np.ones((3,3),np.uint8)
    closing= cv2.morphologyEx(disp,cv2.MORPH_CLOSE, kernel) # Apply an morphological filter for closing little "black" holes in the picture(Remove noise) 

        # Colors map
    dispc= (closing-closing.min())*255
    dispC= dispc.astype(np.uint8)                                   # Convert the type of the matrix from float32 to uint8, this way you can show the results with the function cv2.imshow()
    disp_Color= cv2.applyColorMap(dispC,cv2.COLORMAP_OCEAN)         # Change the Color of the Picture into an Ocean Color_Map
    filt_Color= cv2.applyColorMap(filteredImg,cv2.COLORMAP_OCEAN) 
    #print("filt color shape="+str(filt_Color.shape))
    #print("dispC shape="+str(dispC.shape))
    #print("distance shape="+str(distance.shape))
        # Show the result for the Depth_image
        #cv2.imshow('Disparity', disp)
        #cv2.imshow('Closing',closing)
        #cv2.imshow('Color Depth',disp_Color)



    points=cv2.reprojectImageTo3D(dispC, Q)
    #file1 = open("points.txt","w")
    #for x in points:
    #    print()
    #    for y in x:
    #        for z in y:
                
    #file1.close()
    #print(points.shape)
    #print(points)

    """
    ply_header = '''ply
format ascii 1.0
element vertex %(vert_num)d
property float x
property float y
property float z
property uchar red
property uchar green
property uchar blue
end_header
'''



    print('generating 3d point cloud...',)
    points=cv2.reprojectImageTo3D(dispC, Q)
    print(points.shape)
    #print(points)
    #points = cv2.reprojectImageTo3D(dispC, Q)
    colors = cv2.cvtColor(filteredImg, cv2.COLOR_BGR2RGB)
    mask = dispC > dispC.min()
    out_points = points[mask]
    out_colors = colors[mask]
    out_fn = 'out.ply'
    #write_ply(out_fn, out_points, out_colors)
    #print('%s saved' % out_fn)
    """
    
    def mouse_click(event, x, y, flags, param): 
        # to check if left mouse 
        # button was clicked
        if event == cv2.EVENT_LBUTTONDOWN:
          
        # font for left click event
            
            font = cv2.FONT_HERSHEY_DUPLEX
            LB = 'Left Button'
          
        # display that left button 
        # was clicked.
            cv2.putText(filt_Color, str((x,y)), (x, y), font, 0.4, (0, 0, 255), 2) 
            cv2.putText(filt_Color, str(points[y, x]), (x, y-14), font, 0.4, (0, 0, 255), 2) 
            cv2.putText(filt_Color, str(distance[y,x])+"mm", (x, y-28), font, 0.4, (0, 0, 255), 2) 
            #cv2.resize(filt_Color, (500, 700))
            cv2.imshow('Filtered Color Depth', filt_Color)
            cv2.waitKey(0)

    #cv2.imshow('Filtered Color Depth',filt_Color)
    print("halloo")
    cv2.imwrite('disparity.jpg',filt_Color)
    #print("hallooee")
    break
    #cv2.setMouseCallback("Filtered Color Depth", mouse_click)
    #if cv2.waitKey(1) & 0xFF == ord(' '):
     #   break

#cv2.destroyAllWindows()