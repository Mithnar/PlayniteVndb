﻿using System;
using System.Collections.ObjectModel;
using VndbSharp.Models.Common;

namespace VndbSharp.Models.Producer
{
	public class Producer : ProducerCommon
	{
		public String Language { get; private set; }
		public ProducerLinks Links { get; private set; }
		public ReadOnlyCollection<String> Aliases { get; private set; }
		public String Description { get; private set; }
		public ReadOnlyCollection<Relationship> Relations { get; private set; }
	}
}
