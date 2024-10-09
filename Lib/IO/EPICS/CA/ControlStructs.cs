using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Convergence.IO.EPICS.CA
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DBR_CTRL_INT
	{
		public Int16 status;         /* status of value */
		public Int16 severity;       /* severity of alarm */
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = ChannelAccessConstants.MAX_UNITS_SIZE)]
		public char[] units; /* units of value */
		public Int16 upper_disp_limit;   /* upper limit of graph */
		public Int16 lower_disp_limit;   /* lower limit of graph */
		public Int16 upper_alarm_limit;
		public Int16 upper_warning_limit;
		public Int16 lower_warning_limit;
		public Int16 lower_alarm_limit;
		public Int16 upper_ctrl_limit;   /* upper control limit */
		public Int16 lower_ctrl_limit;   /* lower control limit */
		public Int16 value;          /* current value */
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DBR_CTRL_SHORT
	{
		public Int16 status;         /* status of value */
		public Int16 severity;       /* severity of alarm */
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = ChannelAccessConstants.MAX_UNITS_SIZE)]
		public char[] units; /* units of value */
		public Int16 upper_disp_limit;   /* upper limit of graph */
		public Int16 lower_disp_limit;   /* lower limit of graph */
		public Int16 upper_alarm_limit;
		public Int16 upper_warning_limit;
		public Int16 lower_warning_limit;
		public Int16 lower_alarm_limit;
		public Int16 upper_ctrl_limit;   /* upper control limit */
		public Int16 lower_ctrl_limit;   /* lower control limit */
		public Int16 value;          /* current value */
	};

	/* structure for a control floating point field */
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DBR_CTRL_FLOAT
	{
		public Int16 status;         /* status of value */
		public Int16 severity;       /* severity of alarm */
		public Int16 precision;      /* number of decimal places */
		public Int16 RISC_pad;       /* RISC alignment */
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = ChannelAccessConstants.MAX_UNITS_SIZE)]
		public char[] units; /* units of value */
		//units[MAX_UNITS_SIZE]; /* units of value */
		public Double upper_disp_limit;   /* upper limit of graph */
		public Double lower_disp_limit;   /* lower limit of graph */
		public Double upper_alarm_limit;
		public Double upper_warning_limit;
		public Double lower_warning_limit;
		public Double lower_alarm_limit;
		public Double upper_ctrl_limit;   /* upper control limit */
		public Double lower_ctrl_limit;   /* lower control limit */
		public Double value;          /* current value */
	};

	public unsafe struct EnumStrings
	{
		public fixed char strs[ChannelAccessConstants.MAX_ENUM_STATES*ChannelAccessConstants.MAX_ENUM_STRING_SIZE];  // Simulate 2D array with a single fixed array

		// Accessor function to treat it like a 2D array
		public char this[int i, int j]
		{
			get
			{
				return strs[i * 26 + j];  // 2D access simulated with 1D indexing
			}
			set
			{
				strs[i * 26 + j] = value;  // Set value at (i, j)
			}
		}
		// Indexer to allow assigning a string to a row
		public string this[int row]
		{
			set
			{
				if (value.Length > 26) throw new ArgumentException("String too long");

				// Copy the string into the specified row
				for (int i = 0; i < value.Length; i++)
				{
					strs[row * 26 + i] = value[i];
				}

				// Fill the rest of the row with null characters if the string is shorter
				for (int i = value.Length; i < 26; i++)
				{
					strs[row * 26 + i] = '\0';  // Null character to mark the end of the string
				}
			}
		}
	}

	/* structure for a control enumeration field */
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DBR_CTRL_ENUM
	{
		public Int16 status;         /* status of value */
		public Int16 severity;       /* severity of alarm */
		public Int16 no_str;         /* number of strings */
		public EnumStrings strs; /* state strings */
		public Int16 value;       /* current value */
	};

	/* structure for a control long field */
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DBR_CTRL_LONG
	{
		public Int16 status;         /* status of value */
		public Int16 severity;       /* severity of alarm */
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = ChannelAccessConstants.MAX_UNITS_SIZE)]
		public char[] units; /* units of value */
		public Int32 upper_disp_limit;    /* upper limit of graph */
		public Int32 lower_disp_limit;    /* lower limit of graph */
		public Int32 upper_alarm_limit;
		public Int32 upper_warning_limit;
		public Int32 lower_warning_limit;
		public Int32 lower_alarm_limit;
		public Int32 upper_ctrl_limit;    /* upper control limit */
		public Int32 lower_ctrl_limit;    /* lower control limit */
		public Int32 value;           /* current value */
	};

	/* structure for a control double field */
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DBR_CTRL_DOUBLE
	{
		public Int16 status;         /* status of value */
		public Int16 severity;       /* severity of alarm */
		public Int16 precision;      /* number of decimal places */
		public Int16 RISC_pad0;      /* RISC alignment */
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = ChannelAccessConstants.MAX_UNITS_SIZE)]
		public char[] units; /* units of value */
		public Double upper_disp_limit;  /* upper limit of graph */
		public Double lower_disp_limit;  /* lower limit of graph */
		public Double upper_alarm_limit;
		public Double upper_warning_limit;
		public Double lower_warning_limit;
		public Double lower_alarm_limit;
		public Double upper_ctrl_limit;  /* upper control limit */
		public Double lower_ctrl_limit;  /* lower control limit */
		public Double value;         /* current value */
	};
}
