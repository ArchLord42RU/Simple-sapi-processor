# Simple-sapi-processor
Simple sapi processor with command line interface

Easy to use command line utility for generating wav files from text using windows TTS API

Options:

-i InputFile (string) Input file to be processed.

-t Text (string) Input text to be processed.

-o Path (string) Output file name or full path.

-f Folder (string) Output folder path.

-l VoicesList (string) Display list of all installed voices.

-v VoiceName (string) Voice name.

-s SampleRate (int) Sample rate.

-b BitRate (int) Bit rate. 1 for 8000, 2 for 16000

-c Channel (int) Channel. 1 for stereo, 2 for mono

also any option can be set by defaul by putting into Configuration.ini file.

for example

VoiceName=Nuance Automotive Milena Premium High 22kHz
Channel=0
bitRate=1
sampleRate=16000
folder=\\10.1.100.2\hdd\files\driver_ivr