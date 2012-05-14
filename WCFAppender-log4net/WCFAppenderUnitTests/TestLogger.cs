﻿using System;
using System.Diagnostics;
using System.ServiceModel;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCFAppender_log4net.Interface;

namespace WCFAppenderUnitTests
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class TestLogger : IWCFLogger
	{
		private static string[] _LastLogOutput;


		public static string[] LastLogOutput
		{
			get
			{
				return _LastLogOutput;
			}
			set
			{
				_LastLogOutput = value;
			}
		}
		public void Append(LoggingEventWrapper[] logEvents)
		{
			LastLogOutput = new string[logEvents.Length];
			for(int i=0;i<logEvents.Length;i++)
			{
				LoggingEvent ev = logEvents[i].GetReconstructedLoggingEvent(null);
				LastLogOutput[i] = "SERVER - " + ev.RenderedMessage;
			}
		}

		public void Append(string[] logEntries)
		{
			LastLogOutput = logEntries;
		}
	}
}



/*
 *  Copyright © 2012 edwinf (https://github.com/edwinf)
 *  
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
*/