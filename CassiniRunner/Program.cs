using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;

namespace Cassini
{
  class Program
  {
    static Server _server;
    static string _appPath;
    static int _portNumber;
    static string _virtRoot;

    static void Main(string[] args)
    {
      ParseArgs(args);

      var mutex = new Mutex(false, "mycassini" + _portNumber);
      var mutex2 = new Mutex(false, "mycassinirunning" + _portNumber);

      if (_appPath == "stop")
      {
        mutex.WaitOne();
        mutex2.WaitOne();

        mutex2.ReleaseMutex();
        mutex.ReleaseMutex();
        return;
      }

      _server = new Cassini.Server(_portNumber, _virtRoot, _appPath);

      try
      {
        mutex2.WaitOne();
        _server.Start();
      }
      catch (Exception er)
      {
        Console.WriteLine(er);
      }

      while (mutex.WaitOne(0))
      {
        mutex.ReleaseMutex();
        Thread.Sleep(50);
      }

      mutex2.ReleaseMutex();
    }

    private static void ParseArgs(String[] args)
    {
      string portString = null;
      try
      {
        if (args.Length >= 1)
          _appPath = args[0];

        if (args.Length >= 2)
          portString = args[1];

        if (args.Length >= 3)
          _virtRoot = args[2];
      }
      catch
      {
      }

      if (portString == null)
        portString = "80";

      _portNumber = Int32.Parse(portString);
      if (_virtRoot == null)
        _virtRoot = "/";

      if (_appPath == null)
        _appPath = String.Empty;
    }

  }
}
