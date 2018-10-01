Log4net Extension Sample

This sample demonstrates how to use the ReflectInsight Log4net Extension. The purpose of this sample is to show you how to leverage ReflectInsight with an pre-existing logging framework.

This sample also uses satellite configuration files, which are a practice that we recommend doing. For more information about satellite configuration files, please
see the satellite configuration sample and/or our online resource.

To get started with the Log4net Extension, please do the following:

1. Start by adding an application configuration file (app.config) to your project. If you already have one, then proceed to step #2.

2. Next add/update the configSections with a section for for "insightSettings":

  <configSections>
    <section name="insightSettings" type="ReflectSoftware.Insight.ConfigurationHandler,ReflectSoftware.Insight"/>
  </configSections>

3. Then add a new section called <insightSettings></insightSettings> and add the externalConfigSource to point to your external ReflectInsight.config file:

  <insightSettings externalConfigSource="ReflectInsight.config" />

4. Please make sure you update the ReflectInsight.config file property 'Copy to Output Directory' to 'Copy always'.
  
5. Next ensure you have configured you application with the applicable log4net configuration.

6. In your log4net configuration, add a new appender for the ReflectInsight Log4net extension:

   <appender name="MyLogAppender1" type="ReflectSoftware.Insight.Extensions.Log4net.LogAppender, ReflectSoftware.Insight.Extensions.Log4net">
     <param name="InstanceName" value="log4netInstance1" />
     <param name="DisplayLevel" value="true" />
     <param name="DisplayLocation" value="true" />
   </appender>

7. Now update your log4net configuration <root> section with a reference to the ReflectInsight Log4net appender you just added in step #6.

   <root>
     <appender-ref ref="MyLogAppender1" />
   </root>

8. Finally, you will need to configure the ReflectInsight.config file to hookup to the log4net appender. The following is the ReflectInsight.config file
   configuration. The section that I would like to highlight is the <logManager> and <instance>. Notice that the instance being used
   is the same name as the log4net appender.

	<?xml version="1.0" encoding="utf-8" ?>
	<configuration>
		<insightSettings>
		<baseSettings>
			<configChange enabled="true" />
			<propagateException enabled="false" />
			<exceptionEventTracker time="20" />
			<debugMessageProcess enabled="true" />
		</baseSettings>

		<listenerGroups active="Debug">
			<group name="Debug" enabled="true" maskIdentities="false">
			<destinations>
				<destination name="Viewer" enabled="true" filter="" details="Viewer" />
			</destinations>
			</group>
		</listenerGroups>

		<logManager>
			<instance name="log4netInstance1" category="Log4net1" />
		</logManager>
		</insightSettings>
	</configuration>

9. That's it your done!

Additional Resources

Getting started with the Log4net configuration
https://reflectsoftware.atlassian.net/wiki/spaces/RI5/pages/5570582/Log4net+Extension

Knowledge Base
http://reflectsoftware.uservoice.com/knowledgebase

User Feedback
http://reflectsoftware.uservoice.com/forums/158277-reflectinsight-feedback

Support
support@reflectsoftware.com