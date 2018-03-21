# Oulu 3D Sensor Game Jam


- [Oulu 3D Sensor Game Jam](https://sensorgamejam.com)
- [Sensor sample from Rohm](https://bitbucket.org/Kionix-Rohm/sensor-game-jam-18/src)
- [Github repository](https://github.com/joonakeskitalo/Oulu3DGameJam2018)


**Members**

- Joona Keskitalo
- Mariia Bogdanova
- Kalle Sirviö
- Toni Närhi
- Kimmo Raappana
- Lasse Leinonen

**Unity version**

- [Unity 5.6.5 32bit](https://download.unity3d.com/download_unity/2cac56bf7bb6/Windows32EditorInstaller/UnitySetup32-5.6.5f1.exe) F1 for it to work with the Rohm sensors.




***

## Game Idea






***

**Rohm IoT Game Platform**

- Raspberry Pi 3 is running the Rohm IoT gateway software
	- Communicates with the Kionix IoT Nodes over BLE
	- Sensor data is streamed to TCP/IP sockets
	- 1-5 nodes supported with 100Hz
- PC (Unity) uses the socket data
	- Sensor fusion is executed in Unity to get the quaternion data
- **Control dashboard**
	- Game platform is controlled with the Dashboard: http://192.168.255.2:1880/ui
	- Kionix IoT Node configuration
	- Start / Stop the gateway
	- Raspberry Shutdown / Reboot
- **Startup steps**
	- Plug in the LAN cable between Raspberry and PC
	- Plug in the power to Raspberry
	- Power on your Kionix IoT Nodes
	- short press of the button in the small hole with the ”töks” – tool
	- http://192.168.255.2:1880/ui
	- Press ”START GATEWAY” -  button
	- Check that all Kionix IoT Node leds turn from red to green
	-  Sensor data is now streamed to Unity
- **Orientation reset**
	- Kionix IoT Node orientation must be resetted before starting the game
	- Hold the node horizontally Kionix logo facing to you and press space bar.

<!--  -->

- Battery of the Kionix IoT Node lasts ~6 hours (depends on the battery condition)
- Remember to charge them time to time by plugging in the USB cable
- It makes sense to keep them plugged in whenever possible
- When the Kionix IoT Node battery runs out, the gateway must be stopped and started again
- Shutdown the Raspberry from the Dashboard with ”SHUTDOWN RASPBERRY” – button
- Just taking off the power cable can sometimes mess up the memory card

<!--  -->

- Kionix-Windows-Sensor-Evaluation-Software can be used for the checking the sensor data in detail
- Stream with the name ”KXG08_KMX62_ODR_100” has the same sensor data which is streamed to Unity
- Kionix IoT Evaluation Kit Users Guide.pdf contains information how to use the tool
- NOTE: Only the Kionix IoT Nodes marked with red can be used with this application
- We have reserved four pieces of the Kionix IoT Nodes per team
- we have some spare ones in case of failures









