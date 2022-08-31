*# CommandSender
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

# Advanced
The command Sender currently deals with literal strings, backed by html encoding for special characters. 
Microsoft provides some great examples of the differences between regular strings and literal strings. 
https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/verbatim

string regular_string = "He said, \"This is the last \u0063hance\x0021\"";
string literal_string = @"He said, ""This is the last \u0063hance\x0021""";

Console.WriteLine(regular_string);
Console.WriteLine(literal_string);
// The example displays the following output:
//     He said, "This is the last chance!"
//     He said, "This is the last \u0063hance\x0021"

Similarly, this also applies to characters such as Line Feeds, carriage returns, non-printable characters. 
Example: \r, \n, \t.

When using a program such as Packet Sender, and many others, it deals with regular strings. 
This means something like "Hello\nWorld" would appear like 
string test = "Hello\nWorld"
would appears as 
"Hello
World"

Command Sender currently makes use of literal strings. 
This means, if you try to send "Hello\nWorld", it would appear as 
"Hello\nWorld"


Command Sender makes use of an html decoding that allows us to handle these special characters. 
When looking at the string "Hello\nWorld", we have one special character being '\n'. 
If we wanted to send this command in CommandSender with that newline character, we'd have to use its html encoding. 
Therefore, it changes from 
"Hello\nWorld" to "Hello&#10;World"

You can find special characters and their html encoding here: 
https://www.ascii-code.com/


# Test
 The UDPListener located at https://github.com/Zayik/UDPListener can be used to test if the plugin buttons are sending commands. 
