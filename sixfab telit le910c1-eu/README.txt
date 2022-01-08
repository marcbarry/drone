https://docs.sixfab.com/docs/raspberry-pi-3g-4g-lte-base-hat-troubleshooting
https://docs.sixfab.com/page/sending-at-commands
https://docs.sixfab.com/page/internet-connection-with-telit-le910c1-module-using-ecm-mode

$ minicom -D /dev/ttyUSB2 -b 115200

AT
AT#USBCFG=4								; start to configure the modem for ECM mode, may reboot
AT+CGDCONT=1,"IP","giffgaff.com"		; configure APN
AT#REBOOT					
AT#ECM=1,0								; start the internet connection
										; on reboots network loss, re-run

$ ifconfig wwan0
