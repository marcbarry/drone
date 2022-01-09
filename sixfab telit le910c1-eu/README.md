# Sixfab Base HAT

## Telit LE910C1-EU

- https://www.giffgaff.com/help/articles/internet-apn-settings-guide
- https://docs.sixfab.com/page/sending-at-commands
- https://docs.sixfab.com/page/internet-connection-with-telit-le910c1-module-using-ecm-mode

```
$ minicom -D /dev/ttyUSB2 -b 115200
```

```
AT
AT#USBCFG=4					; start to configure the modem for ECM mode, may reboot
AT+CGDCONT=1,"IP","giffgaff.com"		; configure APN
AT#REBOOT					
AT#ECM=1,0					; start the internet connection
						; on reboots or network/signal loss, re-run
```

```
$ ifconfig wwan0
$ ping -I wwan0 8.8.8.8
```

## Troubleshooting

- https://docs.sixfab.com/docs/raspberry-pi-3g-4g-lte-base-hat-troubleshooting

```
AT+CPIN?					; device can recognize the SIM
AT+CGREG?					; registration status of the device
AT+CSQ						; signal strength of the device
```

### Registration state

```
0,0 Not registered, ME is not currently searching a new operator to register to
0,1 Registered, home network
0,2 Not registered, but ME is currently searching a new operator to register to
0,3 Registration denied
0,4 Unknown
0,5 Registered, Roaming
```

### Signal Quality

```
<rssi> - received signal strength indication
0 - (-113) dBm or less
1 - (-111) dBm
2..30 - (-109)dBm..(-53)dBm / 2 dBm per step
31 - (-51)dBm or greater
99 - not known or not detectable

<ber> - bit error rate (in percent)
0 - less than 0.2%
1 - 0.2% to 0.4%
2 - 0.4% to 0.8%
3 - 0.8% to 1.6%
4 - 1.6% to 3.2%
5 - 3.2% to 6.4%
6 - 6.4% to 12.8%
7 - more than 12.8%
99 - not known or not detectable
```



