# CommandSender
 Stream Deck plugin for sending UDP messages from a key press. 

# Install
 Run the com.biffmasterzay.commandsender.streamDeckPlugin from the Release folder.
 This will open the Elgato StreamDeck program and prompt whether you want to install it or not.
 
# Use
 Once installed on the Stream Deck, add the button. 
 Fill out thre four fields:
	IP Address: (127.0.0.1 is local host)
	Port: #
	Command Pressed: The message/command to send to the server on key press.
	Command Released: The message/command to send to the server on key release.

# Test
 The UDPListener located at https://github.com/Zayik/UDPListener can be used to test if the plugin buttons are sending commands. 


NOTE: Original implementation used an html decoding which required special characters to be listed with their html representation. 
/n --> &#010;

This is no longer required. 
User should now be able to use special characters and have them processed appropriately. 
This means sending "Hello\nWorld" will appear as 
"Hello
World"

instead of 

"Hello\nWorld"
