using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using CommandLine.Text;
using CommandLine;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Reflection;

namespace SAPIProcessor
{
    class Program
    {
        static void Main(string[] args)
        {

            Options options = new Options();

            FileIniDataParser parser = new FileIniDataParser();

            IniData data = null;

            string fullPath = String.Format("{0}/Configuration.ini", Directory.GetCurrentDirectory());

            if (File.Exists(fullPath)){
                data = parser.ReadFile(fullPath);
            }

            bool confReceived = false;

            if(data != null)
            {
                PropertyInfo[] propInfos = options.GetType().GetProperties();

                foreach (KeyData keyData in data.Global)
                {
                    string key = keyData.KeyName.ToLower();

                    foreach (PropertyInfo propInfo in propInfos)
                    {
                        if (propInfo.Name.ToLower() == key)
                        {
                            if (propInfo.PropertyType == typeof(String))
                                propInfo.SetValue(options, keyData.Value);
                            else if(propInfo.PropertyType == typeof(int))
                            {
                                int value = 0;
                                if (int.TryParse(keyData.Value, out value))
                                    propInfo.SetValue(options, value);
                            }
                            else if (propInfo.PropertyType == typeof(bool))
                            {
                                bool value = false;
                                if(bool.TryParse(keyData.Value, out value))
                                    propInfo.SetValue(options, value);
                            }
                        }
                    }
                }
                confReceived = !String.IsNullOrEmpty(options.VoiceName) && options.BitRate > 0 && options.SampleRate > 0 && CommandLine.Parser.Default.ParseArguments(args, options);
            } else
            {
                confReceived = CommandLine.Parser.Default.ParseArguments(args, options);
            }

            confReceived = confReceived && ((!String.IsNullOrEmpty(options.Path) && (!String.IsNullOrEmpty(options.InputFile) || !String.IsNullOrEmpty(options.Text))) || options.VoicesList);

            bool result = true;

            if (confReceived)
            {
                if (options.VoicesList)
                {
                    DisplayVoicesList();
                } else
                {
                    result = GenerateFile(options);
                }
                if (!result)
                {
                    Console.WriteLine("An error has occurred");
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine("Options error");
                Environment.Exit(1);
            }
        }

        static void DisplayVoicesList()
        {
            SpeechSynthesizer speaker = new SpeechSynthesizer();
            var list = speaker.GetInstalledVoices();
            foreach (var item in list)
            {
                Console.WriteLine(item.VoiceInfo.Name);
            }
        }

        static bool GenerateFile(Options options)
        {
            AudioBitsPerSample bitRate = options.BitRate == 1 ? AudioBitsPerSample.Eight : AudioBitsPerSample.Sixteen;
            AudioChannel channel = options.Channel == 1 ? AudioChannel.Stereo : AudioChannel.Mono;

            var format = new SpeechAudioFormatInfo(options.SampleRate, bitRate, channel);

            string text = "";

            if (String.IsNullOrEmpty(options.Text))
                text = File.ReadAllText(options.InputFile, Encoding.UTF8);
            else
                text = options.Text;

            string path = String.IsNullOrEmpty(options.Folder) ? options.Path : String.Format("{0}/{1}", options.Folder, options.Path);

            SpeechSynthesizer speaker = new SpeechSynthesizer();

            try
            {
                speaker.SelectVoice(options.VoiceName);
                speaker.SetOutputToWaveFile(path, format);
                speaker.Speak(text);
            }
            catch (Exception)
            {
                return false;
            }      

            return true;
        }

        class Options
        {
            [Option('i', HelpText = "Input file to be processed.")]
            public string InputFile { get; set; }

            [Option('t', HelpText = "Input text to be processed.")]
            public string Text { get; set; }

            [Option('o', HelpText = "Output file name \\ path")]
            public string Path { get; set; }

            [Option('f', HelpText = "Output folder path")]
            public string Folder { get; set; }

            [Option('l', HelpText = "Display list of all installed voices.")]
            public bool VoicesList { get; set; }

            [Option('v', HelpText = "Voice name.")]
            public string VoiceName { get; set; }

            [Option('s', HelpText = "Sample rate.")]
            public int SampleRate { get; set; }

            [Option('b', HelpText = "Bit rate. 1 = 8000 | 2 = 16000")]
            public int BitRate { get; set; }

            [Option('c', HelpText = "Channel 1 = stereo | 2 = mono.")]
            public int Channel { get; set; }

            [ParserState]
            public IParserState LastParserState { get; set; }
        }
    }
}
