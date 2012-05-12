﻿using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using log4net;
using log4net.Config;
using log4net.Layout;
using log4net.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WCFAppender_log4net.Interface;

namespace WCFAppenderUnitTests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestBasicRenderClientSide()
		{
				TestLogger logger = new TestLogger();
				ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

				WCFAppender_log4net.WCFAppender appender = new WCFAppender_log4net.WCFAppender();
				appender.BufferSize = -1;
				appender.Layout = new SimpleLayout();
				appender.LoggingChannel = logger;
				appender.RenderOnClient = true;

				appender.ActivateOptions();

				BasicConfigurator.Configure(rep,appender);

				ILog log = LogManager.GetLogger(rep.Name, "TestBasicPush");
				log.Debug("This is the first message");
				Assert.IsNotNull(TestLogger.LastLogOutput);
				Assert.IsTrue(TestLogger.LastLogOutput.Length == 1);
				Assert.IsTrue(TestLogger.LastLogOutput[0].Trim() == "DEBUG - This is the first message");
		}

		[TestMethod]
		public void TestBasicRenderServerSide()
		{
			TestLogger logger = new TestLogger();
			ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

			WCFAppender_log4net.WCFAppender appender = new WCFAppender_log4net.WCFAppender();
			appender.BufferSize = -1;
			appender.Layout = new SimpleLayout();
			appender.LoggingChannel = logger;
			appender.RenderOnClient = false;

			appender.ActivateOptions();

			BasicConfigurator.Configure(rep, appender);

			ILog log = LogManager.GetLogger(rep.Name, "TestBasicPush");
			log.Debug("This is the first message");
			Assert.IsNotNull(TestLogger.LastLogOutput);
			Assert.IsTrue(TestLogger.LastLogOutput.Length == 1);
			Assert.IsTrue(TestLogger.LastLogOutput[0].Trim() == "SERVER - This is the first message");
		}

		[TestMethod]
		public void TestWCFCreation()
		{
			ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

			WCFAppender_log4net.WCFAppender appender = new WCFAppender_log4net.WCFAppender();
			appender.URL = "http://localhost:8080/logging/testservice.svc";
			appender.ActivateOptions();

			Assert.IsNotNull(appender.LoggingChannel);
			Assert.IsInstanceOfType(appender.LoggingChannel, typeof(IWCFLogger));
			Assert.IsInstanceOfType(appender.LoggingChannel, typeof(IClientChannel));
		}

		[TestMethod]
		public void TestWCFChannelWithClientRender()
		{
			TestLogger logger = new TestLogger();
			using (ServiceHost host = new ServiceHost(logger, new Uri("http://localhost:8080/")))
			{
				host.AddServiceEndpoint(typeof(IWCFLogger), new BasicHttpBinding(), "Logger");
				host.Open();

				Thread.Sleep(3000);
				ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

				WCFAppender_log4net.WCFAppender appender = new WCFAppender_log4net.WCFAppender();
				appender.URL = "http://localhost:8080/logger";
				appender.BufferSize = -1;
				appender.Layout = new SimpleLayout();
				appender.RenderOnClient = true;
				appender.ActivateOptions();

				Assert.IsNotNull(appender.LoggingChannel);
				Assert.IsInstanceOfType(appender.LoggingChannel, typeof(IWCFLogger));
				Assert.IsInstanceOfType(appender.LoggingChannel, typeof(IClientChannel));

				BasicConfigurator.Configure(rep, appender);
				ILog log = LogManager.GetLogger(rep.Name, "TestBasicPush");

				log.Debug("Other side of the Channel!");


				//Wait for the host to process the recieve
				Thread.Sleep(2000);
				
				Assert.IsNotNull(TestLogger.LastLogOutput);
				Assert.IsTrue(TestLogger.LastLogOutput.Length > 0);
				Assert.IsTrue(TestLogger.LastLogOutput[0].Trim() == "DEBUG - Other side of the Channel!");
				host.Close();
			}
		}

		[TestMethod]
		public void TestWCFChannelWithServerRender()
		{
			TestLogger logger = new TestLogger();
			using (ServiceHost host = new ServiceHost(logger, new Uri("http://localhost:8080/")))
			{
				host.AddServiceEndpoint(typeof(IWCFLogger), new BasicHttpBinding(), "Logger");
				host.Open();

				Thread.Sleep(3000);
				ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

				WCFAppender_log4net.WCFAppender appender = new WCFAppender_log4net.WCFAppender();
				appender.URL = "http://localhost:8080/logger";
				appender.BufferSize = -1;
				appender.ActivateOptions();

				Assert.IsNotNull(appender.LoggingChannel);
				Assert.IsInstanceOfType(appender.LoggingChannel, typeof(IWCFLogger));
				Assert.IsInstanceOfType(appender.LoggingChannel, typeof(IClientChannel));

				BasicConfigurator.Configure(rep, appender);
				ILog log = LogManager.GetLogger(rep.Name, "TestBasicPush");

				log.Debug("Other side of the Channel!");


				//Wait for the host to process the recieve
				Thread.Sleep(2000);

				Assert.IsNotNull(TestLogger.LastLogOutput);
				Assert.IsTrue(TestLogger.LastLogOutput.Length > 0);
				Assert.IsTrue(TestLogger.LastLogOutput[0].Trim() == "SERVER - Other side of the Channel!");
				host.Close();
			}
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