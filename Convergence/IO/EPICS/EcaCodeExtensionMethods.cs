//
// EcaCodeExtensionMethods.cs
//
using System.Diagnostics.CodeAnalysis ;

namespace Convergence.IO.EPICS
{

    internal static class EcaCodeExtensionMethods
    {
        // An ECA code of '1' indicates success ... kindof like boolean 'true' ...

        public static bool IsEcaSuccess(
          this int ecaReturnCode
        )
        {
            return CA_EXTRACT_SUCCESS(ecaReturnCode) is true;
        }

        public static bool IsEcaFailure(
          this int ecaReturnCode,
          [NotNullWhen(true)] out EcaSeverity? ecaSeverity,
          [NotNullWhen(true)] out EcaType? ecaMessage
        )
        {
            if (CA_EXTRACT_SUCCESS(ecaReturnCode) is true)
            {
                // Success, ie no failure to report
                // Hmm, sometimes we still get an informational message ??? *************************
                ecaSeverity = null;
                ecaMessage = null;
                return false;
            }
            else
            {
                // Failed ...
                ecaSeverity = (EcaSeverity)CA_EXTRACT_SEVERITY(ecaReturnCode);
                ecaMessage = (EcaType)CA_EXTRACT_MSG_NO(ecaReturnCode);
                return true;
            }
        }

        public static bool IsEcaFailure(
          this int ecaReturnCode,
          [NotNullWhen(true)] out string? message
        )
        {
            if (CA_EXTRACT_SUCCESS(ecaReturnCode) is true)
            {
                // Success, ie no failure to report
                // Hmm, sometimes we still get a message ???
                // TODO : LOG THE MESSAGE ???
                message = null;
                return false;
            }
            else
            {
                // Failed ...
                var ecaSeverity = (EcaSeverity)CA_EXTRACT_SEVERITY(ecaReturnCode);
                var ecaMessage = (EcaType)CA_EXTRACT_MSG_NO(ecaReturnCode);
                message = $"{ecaMessage} (severity:{ecaSeverity})";
                return true;
            }
        }

        // Extract useful fields from an ECA_ code

        private static bool CA_EXTRACT_SUCCESS(int eca_code)
        => (
          eca_code & ChannelAccessConstants.CA_M_SUCCESS
        ) == ChannelAccessConstants.CA_M_SUCCESS;

        private static int CA_EXTRACT_MSG_NO(int eca_code)
        => (
          (eca_code & ChannelAccessConstants.CA_M_MSG_NO)
        >> ChannelAccessConstants.CA_V_MSG_NO
        );

        private static int CA_EXTRACT_SEVERITY(int eca_code)
        => (
          eca_code & ChannelAccessConstants.CA_M_SEVERITY
        );
    }
}
