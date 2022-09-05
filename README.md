# CommandSender
 Stream Deck plugin for sending UDP messages from a key press. 

# Install
 Run the com.biffmasterzay.commandsender.streamDeckPlugin from the Release folder.
 This will open the Elgato StreamDeck program and prompt whether you want to install it or not.
 
# Use
 Once installed on the Stream Deck, add the button. 
 Fill out the four fields:
	IP Address: (127.0.0.1 is local host)
	Port: #
	Command Pressed: The message/command to send to the server on key press.
	Command Released: The message/command to send to the server on key release.

# Test
 The UDPListener located at https://github.com/Zayik/UDPListener can be used to test sending commands over UDP
 
 The TCPListener located at https://github.com/Zayik/TCPListener can be used to test sending commands over TCP


# FAQS
Q: I've set up the Command Sender button and have a TCP server set up, however, I am still unable to receive messages. What is wrong?
A: The most likely culprit is that there are multiple devices on the receiving network. When this occurs, the router may not know what device to forward the message to. Ensure port forwarding is setup in this scenario to forward messages on select ip/port to the correct device. 

# Donate
If this plugin has helped you out and you think it has value, feel free to send a donation of any amount. It's been a lot of fun working on this and I hope you all have enjoyed using it!

[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?business=2CMZ24E89WANG&no_recurring=0&currency_code=USD)
