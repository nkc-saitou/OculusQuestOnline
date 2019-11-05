using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Matsumoto.GoogleSpreadSheetLoader;

namespace Matsumoto.GoogleSpreadSheetLoader {

	public class SpreadSheet {

		public string Range;
		public string MajorDimension;
		public List<List<string>> Values;
	}

}
