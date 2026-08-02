using Newtonsoft.Json;
using System;

namespace TestBootstrap.Infrastructure
{
    public class DdiConnectionInfo
    {
        public enum DdiIds
        {
            MI_DDI_ID_DEFAULT = 0,
            MI_DDI_ID_NONE = 0,
            MI_DDI_ID_AP4 = 1,
            MI_DDI_ID_SLY = 2,
            MI_DDI_ID_PRO = 3,
            MI_DDI_ID_370 = 4,
            MI_DDI_ID_ORA = 5,
            MI_DDI_ID_DBM = 6,
            MI_DDI_ID_DB22 = 7,
            MI_DDI_ID_ODBC = 8,
            MI_DDI_ID_LOCAL = 9,
            MI_DDI_ID_SQL = 10,
            MI_DDI_ID_DSS = 11, // DataSet server interface
            MI_DDI_ID_DIC = 12, // Data Model
            MI_DDI_ID_SLY_NEW = 13, // new Sample DB (ODBC)
            MI_DDI_ID_DRDA = 14,
            MI_DDI_ID_IP4 = 15,
            MI_DDI_ID_SYB = 16, // Sybase
            MI_DDI_ID_SEC = 17,
            MI_DDI_ID_DDIX2DDI = 18, //
            MI_DDI_ID_UDB = 19,
            MI_DDI_ID_DDID7DCC = 20, //
            MI_DDI_ID_DDID7WFS = 21, //
            MI_DDI_ID_IP4_PCSSQL = 22, // ~ AP4, mode PCSSQL only, fait passer de l'APPC dans du TCP/IP
            MI_DDI_ID_DDID7WFV = 23, //
            MI_DDI_ID_WEBSERVICES = 24,
            MI_DDI_ID_ADOMD = 25,
            MI_DDI_ID_OLEDB = 26,
            MI_DDI_ID_SAP = 27,
            MI_DDI_ID_WEBMENU = 28,
            MI_DDI_ID_TERADATA = 29,
            MI_DDI_ID_BIGQUERY = 30,
            MI_DDI_ID_POWERBI = 31,
            MI_DDI_ID_M3 = 32,
            MI_DDI_ID_BUILDER = 33,
            MI_DDI_ID_TABULAR_DATA_SAMPLE_PROVIDER = 34,
            MI_DDI_ID_SALES_FORCE = 35,
            MI_DDI_ID_DYNAMIC_CODE = 36,
            MI_DDI_ID_MINIMALISTIC = 37, // very basic (hopefully simple) example
            MI_DDI_ID_BUILDERMODEL = 39,
            MI_DDI_ID_RESERVED_FIRST = 40,
            MI_DDI_ID_RESERVED_LAST = 10000,

            MI_DDI_ID_CUSTOM_FIRST = 20000,
            MI_DDI_ID_CUSTOM_LAST = 30000,
        }

        [JsonProperty]
        public DdiIds DDIId { get; set; }

        // Les séparateurs d'ID dans les noms de tables
        [JsonProperty]
        public char[] Separators { get; set; }

        // Column sizes (field size/ comment size)
        [JsonProperty]
        public int SizeFieldName { get; set; }
        [JsonProperty]
        public int SizeComment { get; set; }

        // min/max level tree=> 
        // nivo mini pour les colonnes + nivo maxi pour l'arbre dans DataSet
        [JsonProperty]
        public int MinLevel { get; set; } // 1 based
        [JsonProperty]
        public int MaxLevel { get; set; } // 1 based

        // Les capabilities de l'interface
        public object Capabilities { get; set; }

        // Parsing infos
        public object ParserRules { get; set; }

        [JsonProperty]
        public string NativeDateFormat { get; set; }
        [JsonProperty]
        public string NativeTimeFormat { get; set; }
        [JsonProperty]
        public string NativeTimeStampFormat { get; set; }
        [JsonProperty]
        public bool DateLikeNum { get; set; } // For DataSet (avoid too many changes in it!)

        // FCT File (file with database functions)
        // For DataSet (avoid too many changes in it!)
        // ----------------------------------------------
        [JsonProperty]
        public string FCTFile { get; set; }

        /*
		*************
		* VERSION 3 *
		*************
		*/
        // Text to use for std deviation, variance functions
        [JsonProperty]
        public string StdDevText { get; set; }
        [JsonProperty]
        public string StdDevPText { get; set; }
        [JsonProperty]
        public string VarText { get; set; }
        [JsonProperty]
        public string VarPText { get; set; }

        /*
		  *************
		  * VERSION 4 *
		  *************
		*/
        [JsonProperty]
        public char EscapeChar { get; set; }

        public DdiConnectionInfo()
        {
            // init with default values
            DDIId = DdiIds.MI_DDI_ID_DEFAULT;
            Separators = new char[] { '.', '.' };

            SizeFieldName = 128;
            SizeComment = 128;

            MinLevel = 2;
            MaxLevel = 3;

            //Capabilities = new CapabilitySource();
            //ParserRules = new DdiSmallParserRules();

            NativeDateFormat = "{\"d\" 'yyyy-mm-dd'}";
            NativeTimeFormat = "{t 'HH:MM:SS'}";
            NativeTimeStampFormat = "{ts 'yyyy-mm-dd HH:MM:SS.NNNNNN'}";
            DateLikeNum = false;

            FCTFile = "SQLSERV.FCT";

            // Text to use for std deviation, variance functions
            StdDevText = "";
            StdDevPText = "";
            VarText = "";
            VarPText = "";

            EscapeChar = '\0';

        }
    }
}
