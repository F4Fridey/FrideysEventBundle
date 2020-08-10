using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smod2;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.Lang;
using Smod2.Piping;

namespace FrideysEventBundle
{
	[PluginDetails(
		author = "F4Fridey",
		name = "Frideys Event Bundle",
		description = "A plugin that contains alot of different server events.",
		id = "f4fridey.frideyseventbundle.plugin",
		configPrefix = "feb",
		langFile = "frideyseventbundle",
		version = "1.0",
		SmodMajor = 3,
		SmodMinor = 8,
		SmodRevision = 3
		)]
	public class FrideysEventBundle : Plugin
	{
		[ConfigOption]
		public readonly bool enable = true;

		[ConfigOption]
		public readonly bool auto_add_to_qeue = true;

		[ConfigOption]
		public readonly int normal_rounds_between_auto_event = 4;

		public List<string> eventQeue = new List<string>();
		public int roundState = 0;

		public override void OnDisable()
		{
			this.Info(this.Details.name + " was disabled ):");
		}

		public override void OnEnable()
		{
			this.Info(this.Details.name + " has been loaded :)");
		}

		public override void Register()
		{
			AddEventHandlers(new EventHandler(this));
			AddEventHandlers(new ChaosVsNTFEventHandler(this));
			AddEventHandlers(new PeanutpocalypseEventHandler(this));
			AddEventHandlers(new DClassBattleEventHandler(this));
			AddEventHandlers(new DClassInvasionEventHandler(this));
			AddEventHandlers(new TTTEventHandler(this));
			AddEventHandlers(new DeathmatchEventHandler(this));
			AddCommand("event", new EventCommand(this));
		}
	}
}
