About
=====
DerekWare Iris is a Windows application that allows you to control your LIFX and Philips Hue devices from your desktop, including dynamic animations called Effects and saved settings called Scenes that allow you to quickly apply colors and effects to multiple lights.


Basic Light Control
===================
Find the light you want to control in the tree on the left of the main screen. When you select it, you'll see the Action Panel open. Within that UI, you can turn the device on or off, change the light color, select a color Theme or animate the device with an Effect.

To change the color of the light, select a standard color from the drop-down list such as, "Warm White." You may also manually change the hue, saturation, brightness and kelvin values of the light in the text boxes below the standard color list. You may also click on the color box to the right to bring up the color picker, which is similar to other color pickers in Windows.

Multizone lights, such as the LIFX Z strip, can set multiple colors at the same time. Like the solid color controls, you may click on the color band under, "Zone Colors," to select individual zone colors.


Groups
======
The Philips Hue and LIFX apps that come from the device manufacturer allow you to create groups of lights that can be controlled together (LIFX also provides Locations, which are just another type of group). DerekWare Iris reads these groups from the device or bridge and allows you to control them the same way you would control a single light.

Additionally, if your group has multiple single-zone lights, DerekWare Iris will treat the group as a multizone light, so you can use multizone Themes and Effects. For example, I have a lamp with 6 Philips Hue bulbs. In DerekWare Iris, I can control the lights both individually and as a single 6-zone light.

You may not create or modify groups within DerekWare Iris. Currently, that can only be done in the stock LIFX and Philips Hue mobile apps.


Themes
======
A theme is simply a collection of colors that may be applied with a single button-click. For example, the Spectrum effect sets each color zone in your multizone device to a different hue that maps to the visible spectrum of light. This is particularly cool with large numbers of color zones, such as a 10-meter LIFX Z strip that has 80 color zones.

Currently, all themes are dynamic and while you may adjust some of the settings, you can not yet create and save your own theme. User themes are coming soon.


Effects
=======
An effect is a dynamic animation that modifies the colors of your lights over time. For example, the Move effect shifts the colors in your multizone light so that it looks like your colors are moving. Like themes, each effect has its own set of properties you may modify when you apply the theme.

To test this out, try clicking the Spectrum theme, then click the Move effect.


Scenes
======
A scene is a collection of devices and their properties that may be saved and applied whenever you want. When you create a new scene, the device list on the left will show checkboxes. Find the individual devices or groups that you want to include in the scene and check their box(es). You'll see a tab appear in the action panel that looks similar to what you see when you're modifying a device's properties, but now, you're modifying the scene properties. When you first add a device to the scene, the scene will capture the current state of the device, but you may modify it in the scene editor panel. Once you've added your devices and set their properties, you can apply the scene by clicking, "Scenes => Apply Scene," or just double-clicking the scene name in the list. If you exit the app, the scenes are saved to the settings, so they'll still be there when you restart.


Why Don't I See my Devices?
===========================
DerekWare Iris sends out a network beacon, looking for any LIFX lights to respond. If that works, you'll see your LIFX devices automatically appear in the device list. If they don't, either because your light just ignored the beacon or because your router dropped the beacon network packets (as mine does), you can manually add your devices by IP address under the File menu. Any device the app can successfully connect to will be saved for the next time, so you shouldn't have to keep adding them every time you start the app.

For Philips Hue devices, you must register DerekWare Iris with your bridge. Click, "File => Connect to Hue Bridge," and either find your bridge in the list or enter the IP address manually. Then, walk to your Hue Bridge device and click the big button on top. Walk back to your PC and click OK in the connection dialog and that's it!


Why Don't I See the Themes and Effects from the Mobile Apps?
============================================================
All themes, scenes and effects are built directly into DerekWare Iris and are not related to the ones you can create in the LIFX or Philips Hue mobile apps.


How Do I Get Updates?
=====================
DerekWare Iris automatically checks for updates on application startup, so you'll always have the latest published version. If you leave Iris running all the time and never restart it, you can manually check for updates in the File menu. If there are updates available, a dialog will pop up, prompting you to update. If not, nothing will happen.


Can I See the Source Code?
==========================
https://github.com/DerekWare/Iris


When Is Feature X or a Bug Fix for Y Coming?
============================================
Soon? This is very much a work in progress, but it's a fun hobby, so I work on it pretty frequently.