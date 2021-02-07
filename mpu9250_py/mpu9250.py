# MPU6050 9-DoF Example Printout

from mpu9250_i2c import *

time.sleep(1) # delay necessary to allow mpu9250 to settle

print('recording data')
while 1:
    try:
        ax,ay,az,wx,wy,wz = mpu6050_conv() # read and convert mpu6050 data
        mx,my,mz = AK8963_conv() # read and convert AK8963 magnetometer data
    except:
        continue

    print('accel [g]: x = {0:2.2f}, y = {1:2.2f}, z = {2:2.2f} | gyro [dps]:  x = {3:2.2f}, y = {4:2.2f}, z = {5:2.2f} | mag [uT]: x = {6:2.2f} y = {7:2.2f} z ={8:2.2f}'
	.format(
		ax,
		ay,
		az,
		wx,
		wy,
		wz,
		mx,
		my,
		mz))

#    print('{}'.format('-'*30))
#    print('gyro [dps]:  x = {0:2.2f}, y = {1:2.2f}, z = {2:2.2f}'.format(wx,wy,wz))
#    print('mag [uT]:   x = {0:2.2f}, y = {1:2.2f}, z = {2:2.2f}'.format(mx,my,mz))
#    print('{}'.format('-'*30))
#    time.sleep(1)
