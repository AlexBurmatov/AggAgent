using System;
using System.Collections.Generic;
using System.Text;

namespace com.tibbo.aggregate.common
{
    public class Cres : CheckedResourceBundle
    {
        private static readonly ResourceBundle bundle = fetch();
        private static readonly Encoding locale = Encoding.Default;

        public static ResourceBundle get()
        {
            return bundle ?? fetch();
        }

        private static ResourceBundle fetch()
        {
            return getBundle(typeof(Cres).Name, locale);
        }
    }

    public class CheckedResourceBundle : ResourceBundle
    {
    }

    public class ResourceBundle
    {
        protected static ResourceBundle getBundle(String baseName, Encoding locale)
        {
            return new Cres();
        }

        public String getString(String key)
        {
            var assoc = contents.Find(each => each[0] == key);
            if (assoc == null)
                throw new KeyNotFoundException("\"" + key + "\" was not found in Cres");
            return assoc[1];
        }

        private static readonly List<String[]> contents =
            new List<String[]>(new[]
                                   {
                                       // Generic resources
      
                                       new[] {"absolute", "Absolute"},
                                       new[] {"acknowledgements", "Acknowledgements"},
                                       new[] {"action", "Action"},
                                       new[] {"ag", "AggreGate"},
                                       new[] {"alert", "Alert"},
                                       new[] {"all", "All"},
                                       new[] {"at", "at"},
                                       new[] {"average", "Average"},
                                       new[] {"background", "Background"},
                                       new[] {"by", "by"},
                                       new[] {"of", "of"},
                                       new[] {"cancel", "Cancel"},
                                       new[] {"cause", "Cause"},
                                       new[] {"center", "Center"},
                                       new[] {"close", "Close"},
                                       new[] {"color", "Color"},
                                       new[] {"configuration", "Configuration"},
                                       new[] {"confirmation", "Confirmation"},
                                       new[] {"configure", "Configure"},
                                       new[] {"context", "Context"},
                                       new[] {"custom", "Custom"},
                                       new[] {"data", "Data"},
                                       new[] {"date", "Date"},
                                       new[] {"default", "Default"},
                                       new[] {"delete", "Delete"},
                                       new[] {"description", "Description"},
                                       new[] {"device", "Device"},
                                       new[] {"disabled", "Disabled"},
                                       new[] {"disconnected", "Disconnected"},
                                       new[] {"edit", "Edit"},
                                       new[] {"enabled", "Enabled"},
                                       new[] {"empty", "Empty"},
                                       new[] {"error", "Error"},
                                       new[] {"errors", "Errors"},
                                       new[] {"escape", "Escape"},
                                       new[] {"event", "Event"},
                                       new[] {"export", "Export"},
                                       new[] {"expression", "Expression"},
                                       new[] {"female", "Female"},
                                       new[] {"field", "Field"},
                                       new[] {"fields", "Fields"},
                                       new[] {"format", "Format"},
                                       new[] {"gender", "Gender"},
                                       new[] {"group", "Group"},
                                       new[] {"help", "Help"},
                                       new[] {"hidden", "Hidden"},
                                       new[] {"hide", "Hide"},
                                       new[] {"id", "ID"},
                                       new[] {"image", "Image"},
                                       new[] {"import", "Import"},
                                       new[] {"in", "in"},
                                       new[] {"index", "Index"},
                                       new[] {"info", "Info"},
                                       new[] {"interrupted", "Interrupted"},
                                       new[] {"level", "Level"},
                                       new[] {"ls", "LinkServer"},
                                       new[] {"login", "Login"},
                                       new[] {"male", "Male"},
                                       new[] {"mappings", "Mappings"},
                                       new[] {"maximum", "Maximum"},
                                       new[] {"message", "Message"},
                                       new[] {"minimum", "Minimum"},
                                       new[] {"name", "Name"},
                                       new[] {"na", "N/A"},
                                       new[] {"no", "No"},
                                       new[] {"none", "None"},
                                       new[] {"notice", "Notice"},
                                       new[] {"null", "NULL"},
                                       new[] {"ok", "OK"},
                                       new[] {"online", "Online"},
                                       new[] {"offline", "Offline"},
                                       new[] {"parameter", "Parameter"},
                                       new[] {"parameters", "Parameters"},
                                       new[] {"password", "Password"},
                                       new[] {"properties", "Properties"},
                                       new[] {"property", "Property"},
                                       new[] {"recipient", "Recipient"},
                                       new[] {"register", "Register"},
                                       new[] {"recipients", "Recipients"},
                                       new[] {"relative", "Relative"},
                                       new[] {"replicate", "Replicate"},
                                       new[] {"report", "Report"},
                                       new[] {"server", "Server"},
                                       new[] {"show", "Show"},
                                       new[] {"side", "Side"},
                                       new[] {"sound", "Sound"},
                                       new[] {"source", "Source"},
                                       new[] {"state", "State"},
                                       new[] {"status", "Status"},
                                       new[] {"subject", "Subject"},
                                       new[] {"successful", "Successful"},
                                       new[] {"summation", "Summation"},
                                       new[] {"table", "Table"},
                                       new[] {"target", "Target"},
                                       new[] {"text", "Text"},
                                       new[] {"time", "Time"},
                                       new[] {"timestamp", "Timestamp"},
                                       new[] {"timezone", "Time Zone"},
                                       new[] {"title", "Title"},
                                       new[] {"tracker", "Tracker"},
                                       new[] {"trigger", "Trigger"},
                                       new[] {"type", "Type"},
                                       new[] {"user", "User"},
                                       new[] {"username", "Username"},
                                       new[] {"users", "Users"},
                                       new[] {"value", "Value"},
                                       new[] {"variable", "Variable"},
                                       new[] {"version", "Version"},
                                       new[] {"widget", "Widget"},
                                       new[] {"yes", "Yes"},
                                       new[] {"devNestedAssets", "Nested Assets"},
                                       // Generic terms
                                       new[] {"notSelected", "<Not selected>"},
                                       // Commands
                                       new[] {"cmdDisconnected", "Disconnected"},
                                       new[] {"cmdTimeout", "Timeout while waiting reply to command ''{0}''"},
                                       // Actions
                                       new[]
                                           {
                                               "acActionBeingExecuted",
                                               "This action is already being executed, concurrent execution is not allowed"
                                           },
                                       // Contexts
                                       new[] {"conIconId", "Icon ID"},
                                       new[] {"conVarList", "List of available variables"},
                                       new[] {"conFuncList", "List of available functions"},
                                       new[] {"conEvtList", "List of available events"},
                                       new[] {"conChildList", "List of child contexts"},
                                       new[] {"conContextPath", "Context Path"},
                                       new[] {"conContextType", "Context Type"},
                                       new[] {"conContextProps", "Context Properties"},
                                       new[] {"conCopyProperties", "Copy Properties"},
                                       new[]
                                           {
                                               "conCopyToChildren",
                                               "Copy Properties to Children"
                                           }
                                       ,
                                       new[]
                                           {
                                               "conInfoEvtDesc",
                                               "Information"
                                           },
                                       new[]
                                           {
                                               "conChildAdded", "Child Added"
                                           },
                                       new[]
                                           {
                                               "conChildRemoved",
                                               "Child Removed"
                                           },
                                       new[]
                                           {
                                               "conVarAdded",
                                               "Variable Definition Added"
                                           },
                                       new[]
                                           {
                                               "conFuncAdded",
                                               "Function Definition Added"
                                           },
                                       new[]
                                           {
                                               "conEvtAdded",
                                               "Event Definition Added"
                                           },
                                       new[]
                                           {
                                               "conVarRemoved",
                                               "Variable Definition Removed"
                                           }
                                       ,
                                       new[]
                                           {
                                               "conFuncRemoved",
                                               "Function Definition Removed"
                                           }
                                       ,
                                       new[]
                                           {
                                               "conEvtRemoved",
                                               "Event Definition Removed"
                                           },
                                       new[]
                                           {
                                               "conInfoChanged",
                                               "Context Basic Information Changed"
                                           },
                                       new[]
                                           {
                                               "conAccessDenied",
                                               "Access denied in context ''{0}'': permissions ''{1}'', need ''{2}''"
                                           },
                                       new[]
                                           {
                                               "conNotAvail",
                                               "Context not available: "
                                           },
                                       new[]
                                           {
                                               "conVarNotAvail",
                                               "Variable not available: "
                                           },
                                       new[]
                                           {
                                               "conFuncNotAvail",
                                               "Function not available: "
                                           },
                                       new[]
                                           {
                                               "conEvtNotAvail",
                                               "Event not available: "
                                           },
                                       new[]
                                           {
                                               "conErrGettingVar",
                                               "Error getting variable ''{0}'' from context ''{1}'': "
                                           },
                                       new[]
                                           {
                                               "conVarNotFound",
                                               "Variable ''{0}'' not found in context ''{1}''"
                                           },
                                       new[]
                                           {
                                               "conErrSettingVar",
                                               "Error setting variable ''{0}'' in context ''{1}'': "
                                           },
                                       new[]
                                           {
                                               "conFuncNotFound",
                                               "Function ''{0}'' not found in context ''{1}''"
                                           },
                                       new[]
                                           {
                                               "conFuncNotImpl",
                                               "Function ''{0}'' not implemented in context ''{1}''"
                                           },
                                       new[]
                                           {
                                               "conErrCallingFunc",
                                               "Error calling function ''{0}'' of context ''{1}'': "
                                           },
                                       new[]
                                           {
                                               "conUpdated",
                                               "Context property changed"
                                           },
                                       new[]
                                           {
                                               "conDestroyedPermanently",
                                               "Context destroyed permanently"
                                           },
                                       new[]
                                           {
                                               "conContextMask",
                                               "Context Mask"
                                           },
                                       new[]
                                           {
                                               "conVarNotFoundInTgt",
                                               "Variable not found in target context"
                                           },
                                       new[]
                                           {
                                               "conVarNotWritableInTgt",
                                               "Variable is not writable in target context"
                                           },
                                       new[]
                                           {
                                               "conErrGettingTgtVar",
                                               "Error getting variable from target context: "
                                           },
                                       new[]
                                           {
                                               "conErrSettingTgtVar",
                                               "Error setting variable in target context: "
                                           },
                                       new[]
                                           {
                                               "conElNotDefined",
                                               "Not Defined"
                                           },
                                       new[] {"conElNotice", "Notice"},
                                       new[] {"conElInfo", "Info"},
                                       new[] {"conElWarning", "Warning"},
                                       new[] {"conElError", "Error"},
                                       new[] {"conElFatal", "Fatal"},
                                       new[]
                                           {
                                               "conEventNotFound",
                                               "Event not found: "
                                           },
                                       new[]
                                           {
                                               "conErrAddingListener",
                                               "Error adding listener for event ''{0}'' of context ''{1}''"
                                           },
                                       new[]
                                           {
                                               "conErrRemovingListener",
                                               "Error removing listener for event ''{0}'' of context ''{1}''"
                                           },
                                       new[]
                                           {
                                               "conMakeSelection",
                                               "Make Selection"
                                           },
                                       // Countries
      
                                       new[] {"countryAlbania", "Albania"}
                                       ,
                                       new[] {"countryAlgeria", "Algeria"}
                                       ,
                                       new[]
                                           {
                                               "countryAmerican_Samoa",
                                               "American Samoa"
                                           },
                                       new[] {"countryAndorra", "Andorra"}
                                       ,
                                       new[] {"countryAngola", "Angola"},
                                       new[]
                                           {"countryAnguilla", "Anguilla"}
                                       ,
                                       new[]
                                           {
                                               "countryAntarctica",
                                               "Antarctica"
                                           },
                                       new[]
                                           {
                                               "countryAntigua_and_Barbuda",
                                               "Antigua and Barbuda"
                                           },
                                       new[]
                                           {
                                               "countryArgentina",
                                               "Argentina"
                                           },
                                       new[] {"countryArmenia", "Armenia"}
                                       ,
                                       new[] {"countryAruba", "Aruba"},
                                       new[]
                                           {
                                               "countryAustralia",
                                               "Australia"
                                           },
                                       new[] {"countryAustria", "Austria"}
                                       ,
                                       new[]
                                           {
                                               "countryAzerbaijan",
                                               "Azerbaijan"
                                           },
                                       new[] {"countryBahamas", "Bahamas"}
                                       ,
                                       new[] {"countryBahrain", "Bahrain"}
                                       ,
                                       new[]
                                           {
                                               "countryBangladesh",
                                               "Bangladesh"
                                           },
                                       new[]
                                           {"countryBarbados", "Barbados"}
                                       ,
                                       new[] {"countryBelarus", "Belarus"}
                                       ,
                                       new[] {"countryBelgium", "Belgium"}
                                       ,
                                       new[] {"countryBelize", "Belize"},
                                       new[] {"countryBenin", "Benin"},
                                       new[] {"countryBermuda", "Bermuda"}
                                       ,
                                       new[] {"countryBhutan", "Bhutan"},
                                       new[] {"countryBolivia", "Bolivia"}
                                       ,
                                       new[]
                                           {
                                               "countryBosnia_and_Herzegovina"
                                               , "Bosnia and Herzegovina"
                                           },
                                       new[]
                                           {"countryBotswana", "Botswana"}
                                       ,
                                       new[]
                                           {
                                               "countryBouvet_Island",
                                               "Bouvet Island"
                                           },
                                       new[] {"countryBrazil", "Brazil"},
                                       new[]
                                           {
                                               "countryBritish_Indian_Ocean_Territory"
                                               ,
                                               "British Indian Ocean Territory"
                                           },
                                       new[]
                                           {
                                               "countryBritish_Virgin_Islands"
                                               , "British Virgin Islands"
                                           },
                                       new[]
                                           {
                                               "countryBrunei_Darussalam",
                                               "Brunei Darussalam"
                                           },
                                       new[]
                                           {"countryBulgaria", "Bulgaria"}
                                       ,
                                       new[]
                                           {
                                               "countryBurkina_Faso",
                                               "Burkina Faso"
                                           },
                                       new[] {"countryBurundi", "Burundi"}
                                       ,
                                       new[]
                                           {"countryCambodia", "Cambodia"}
                                       ,
                                       new[]
                                           {"countryCameroon", "Cameroon"}
                                       ,
                                       new[] {"countryCanada", "Canada"},
                                       new[]
                                           {
                                               "countryCape_Verde",
                                               "Cape Verde"
                                           },
                                       new[]
                                           {
                                               "countryCayman_Islands",
                                               "Cayman Islands"
                                           },
                                       new[]
                                           {
                                               "countryCentral_African_Republic"
                                               ,
                                               "Central African Republic"
                                           },
                                       new[] {"countryChad", "Chad"},
                                       new[] {"countryChile", "Chile"},
                                       new[] {"countryChina", "China"},
                                       new[]
                                           {
                                               "countryChristmas_Island",
                                               "Christmas Island"
                                           },
                                       new[]
                                           {
                                               "countryCocos_Islands",
                                               "Cocos (Keeling) Islands"
                                           },
                                       new[]
                                           {"countryColombia", "Colombia"}
                                       ,
                                       new[] {"countryComoros", "Comoros"}
                                       ,
                                       new[] {"countryCongo", "Congo"},
                                       new[]
                                           {
                                               "countryCongoRep",
                                               "Congo, the Democratic Republic of the"
                                           },
                                       new[]
                                           {
                                               "countryCook_Islands",
                                               "Cook Islands"
                                           },
                                       new[]
                                           {
                                               "countryCosta_Rica",
                                               "Costa Rica"
                                           },
                                       new[] {"countryCroatia", "Croatia"}
                                       ,
                                       new[] {"countryCyprus", "Cyprus"},
                                       new[]
                                           {
                                               "countryCzech_Republic",
                                               "Czech Republic"
                                           },
                                       new[] {"countryDenmark", "Denmark"}
                                       ,
                                       new[]
                                           {"countryDjibouti", "Djibouti"}
                                       ,
                                       new[]
                                           {"countryDominica", "Dominica"}
                                       ,
                                       new[]
                                           {
                                               "countryDominican_Republic",
                                               "Dominican Republic"
                                           },
                                       new[]
                                           {
                                               "countryEast_Timor",
                                               "East Timor"
                                           },
                                       new[] {"countryEcuador", "Ecuador"}
                                       ,
                                       new[] {"countryEgypt", "Egypt"},
                                       new[]
                                           {
                                               "countryEl_Salvador",
                                               "El Salvador"
                                           },
                                       new[]
                                           {
                                               "countryEquatorial_Guinea",
                                               "Equatorial Guinea"
                                           },
                                       new[] {"countryEritrea", "Eritrea"}
                                       ,
                                       new[] {"countryEstonia", "Estonia"}
                                       ,
                                       new[]
                                           {"countryEthiopia", "Ethiopia"}
                                       ,
                                       new[]
                                           {
                                               "countryFaeroe_Islands",
                                               "Faeroe Islands"
                                           },
                                       new[]
                                           {
                                               "countryFalkland_Islands",
                                               "Falkland Islands (Malvinas)"
                                           }
                                       ,
                                       new[] {"countryFiji", "Fiji"},
                                       new[] {"countryFinland", "Finland"}
                                       ,
                                       new[] {"countryFrance", "France"},
                                       new[]
                                           {
                                               "countryFrench_Guiana",
                                               "French Guiana"
                                           },
                                       new[]
                                           {
                                               "countryFrench_Polynesia",
                                               "French Polynesia"
                                           },
                                       new[]
                                           {
                                               "countryFrench_Southern_Territories"
                                               ,
                                               "French Southern Territories"
                                           },
                                       new[] {"countryGabon", "Gabon"},
                                       new[] {"countryGambia", "Gambia"},
                                       new[] {"countryGeorgia", "Georgia"}
                                       ,
                                       new[] {"countryGermany", "Germany"}
                                       ,
                                       new[] {"countryGhana", "Ghana"},
                                       new[]
                                           {
                                               "countryGibraltar",
                                               "Gibraltar"
                                           },
                                       new[] {"countryGreece", "Greece"},
                                       new[]
                                           {
                                               "countryGreenland",
                                               "Greenland"
                                           },
                                       new[] {"countryGrenada", "Grenada"}
                                       ,
                                       new[]
                                           {
                                               "countryGuadeloupe",
                                               "Guadeloupe"
                                           },
                                       new[] {"countryGuam", "Guam"},
                                       new[]
                                           {
                                               "countryGuatemala",
                                               "Guatemala"
                                           },
                                       new[] {"countryGuinea", "Guinea"},
                                       new[]
                                           {
                                               "countryGuinea-Bissau",
                                               "Guinea-Bissau"
                                           },
                                       new[] {"countryGuyana", "Guyana"},
                                       new[] {"countryHaiti", "Haiti"},
                                       new[]
                                           {
                                               "countryHeard_and_Mc_Donald_Islands"
                                               ,
                                               "Heard and Mc Donald Islands"
                                           },
                                       new[]
                                           {
                                               "countryHoly_See",
                                               "Holy See (Vatican City State)"
                                           },
                                       new[]
                                           {"countryHonduras", "Honduras"}
                                       ,
                                       new[]
                                           {
                                               "countryHong_Kong",
                                               "Hong Kong"
                                           },
                                       new[] {"countryHungary", "Hungary"}
                                       ,
                                       new[] {"countryIceland", "Iceland"}
                                       ,
                                       new[] {"countryIndia", "India"},
                                       new[]
                                           {
                                               "countryIndonesia",
                                               "Indonesia"
                                           },
                                       new[] {"countryIreland", "Ireland"}
                                       ,
                                       new[] {"countryIsrael", "Israel"},
                                       new[] {"countryItaly", "Italy"},
                                       new[]
                                           {
                                               "countryIvory_Coast",
                                               "Ivory Coast"
                                           },
                                       new[] {"countryJamaica", "Jamaica"}
                                       ,
                                       new[] {"countryJapan", "Japan"},
                                       new[] {"countryJordan", "Jordan"},
                                       new[]
                                           {
                                               "countryKazakhstan",
                                               "Kazakhstan"
                                           },
                                       new[] {"countryKenya", "Kenya"},
                                       new[]
                                           {"countryKiribati", "Kiribati"}
                                       ,
                                       new[]
                                           {
                                               "countryKorea",
                                               "Korea, Republic of"
                                           },
                                       new[] {"countryKuwait", "Kuwait"},
                                       new[]
                                           {
                                               "countryKyrgyzstan",
                                               "Kyrgyzstan"
                                           },
                                       new[]
                                           {
                                               "countryLao",
                                               "Lao People''s Democratic Republic"
                                           },
                                       new[] {"countryLatvia", "Latvia"},
                                       new[] {"countryLebanon", "Lebanon"}
                                       ,
                                       new[] {"countryLesotho", "Lesotho"}
                                       ,
                                       new[] {"countryLiberia", "Liberia"}
                                       ,
                                       new[]
                                           {
                                               "countryLiechtenstein",
                                               "Liechtenstein"
                                           },
                                       new[]
                                           {
                                               "countryLithuania",
                                               "Lithuania"
                                           },
                                       new[]
                                           {
                                               "countryLuxembourg",
                                               "Luxembourg"
                                           },
                                       new[] {"countryMacao", "Macao"},
                                       new[]
                                           {
                                               "countryMacedonia",
                                               "Macedonia"
                                           },
                                       new[]
                                           {
                                               "countryMadagascar",
                                               "Madagascar"
                                           },
                                       new[] {"countryMalawi", "Malawi"},
                                       new[]
                                           {"countryMalaysia", "Malaysia"}
                                       ,
                                       new[]
                                           {"countryMaldives", "Maldives"}
                                       ,
                                       new[] {"countryMali", "Mali"},
                                       new[] {"countryMalta", "Malta"},
                                       new[]
                                           {
                                               "countryMarshall_Islands",
                                               "Marshall Islands"
                                           },
                                       new[]
                                           {
                                               "countryMartinique",
                                               "Martinique"
                                           },
                                       new[]
                                           {
                                               "countryMauritania",
                                               "Mauritania"
                                           },
                                       new[]
                                           {
                                               "countryMauritius",
                                               "Mauritius"
                                           },
                                       new[] {"countryMayotte", "Mayotte"}
                                       ,
                                       new[] {"countryMexico", "Mexico"},
                                       new[]
                                           {
                                               "countryMicronesia",
                                               "Micronesia, Federated States of"
                                           }
                                       ,
                                       new[]
                                           {
                                               "countryMoldova, Republic of",
                                               "Moldova, Republic of"
                                           },
                                       new[] {"countryMonaco", "Monaco"},
                                       new[]
                                           {"countryMongolia", "Mongolia"}
                                       ,
                                       new[]
                                           {
                                               "countryMontenegro",
                                               "Montenegro"
                                           },
                                       new[]
                                           {
                                               "countryMontserrat",
                                               "Montserrat"
                                           },
                                       new[] {"countryMorocco", "Morocco"}
                                       ,
                                       new[]
                                           {
                                               "countryMozambique",
                                               "Mozambique"
                                           },
                                       new[] {"countryNamibia", "Namibia"}
                                       ,
                                       new[] {"countryNauru", "Nauru"},
                                       new[] {"countryNepal", "Nepal"},
                                       new[]
                                           {
                                               "countryNetherlands",
                                               "Netherlands"
                                           },
                                       new[]
                                           {
                                               "countryNetherlands_Antilles",
                                               "Netherlands Antilles"
                                           },
                                       new[]
                                           {
                                               "countryNew_Caledonia",
                                               "New Caledonia"
                                           },
                                       new[]
                                           {
                                               "countryNew_Zealand",
                                               "New Zealand"
                                           },
                                       new[]
                                           {
                                               "countryNicaragua",
                                               "Nicaragua"
                                           },
                                       new[] {"countryNiger", "Niger"},
                                       new[] {"countryNigeria", "Nigeria"}
                                       ,
                                       new[] {"countryNiue", "Niue"},
                                       new[]
                                           {
                                               "countryNorfolk_Island",
                                               "Norfolk Island"
                                           },
                                       new[]
                                           {
                                               "countryNorthern_Mariana_Islands"
                                               ,
                                               "Northern Mariana Islands"
                                           },
                                       new[] {"countryNorway", "Norway"},
                                       new[] {"countryOman", "Oman"},
                                       new[]
                                           {"countryPakistan", "Pakistan"}
                                       ,
                                       new[] {"countryPalau", "Palau"},
                                       new[]
                                           {
                                               "countryPalestinian_Territory"
                                               , "Palestinian Territory"
                                           },
                                       new[] {"countryPanama", "Panama"},
                                       new[]
                                           {
                                               "countryPapua_New_Guinea",
                                               "Papua New Guinea"
                                           },
                                       new[]
                                           {"countryParaguay", "Paraguay"}
                                       ,
                                       new[] {"countryPeru", "Peru"},
                                       new[]
                                           {
                                               "countryPhilippines",
                                               "Philippines"
                                           },
                                       new[]
                                           {"countryPitcairn", "Pitcairn"}
                                       ,
                                       new[] {"countryPoland", "Poland"},
                                       new[]
                                           {"countryPortugal", "Portugal"}
                                       ,
                                       new[]
                                           {
                                               "countryPuerto_Rico",
                                               "Puerto Rico"
                                           },
                                       new[] {"countryQatar", "Qatar"},
                                       new[] {"countryReunion", "Reunion"}
                                       ,
                                       new[] {"countryRomania", "Romania"}
                                       ,
                                       new[]
                                           {
                                               "countryRussian_Federation",
                                               "Russian Federation"
                                           },
                                       new[] {"countryRwanda", "Rwanda"},
                                       new[]
                                           {
                                               "countrySaint_Helena",
                                               "Saint Helena"
                                           },
                                       new[]
                                           {
                                               "countrySaint_Kitts_and_Nevis"
                                               , "Saint Kitts and Nevis"
                                           },
                                       new[]
                                           {
                                               "countrySaint_Lucia",
                                               "Saint Lucia"
                                           },
                                       new[]
                                           {
                                               "countrySaint_Pierre_and_Miquelon"
                                               ,
                                               "Saint Pierre and Miquelon"
                                           },
                                       new[]
                                           {
                                               "countrySaint_Vincent_and_the_Grenadines"
                                               ,
                                               "Saint Vincent and the Grenadines"
                                           },
                                       new[] {"countrySamoa", "Samoa"},
                                       new[]
                                           {
                                               "countrySan_Marino",
                                               "San Marino"
                                           },
                                       new[]
                                           {
                                               "countrySao_Tome_and_Principe"
                                               , "Sao Tome and Principe"
                                           },
                                       new[]
                                           {
                                               "countrySaudi_Arabia",
                                               "Saudi Arabia"
                                           },
                                       new[] {"countrySenegal", "Senegal"}
                                       ,
                                       new[] {"countrySerbia", "Serbia"},
                                       new[]
                                           {
                                               "countrySeychelles",
                                               "Seychelles"
                                           },
                                       new[]
                                           {
                                               "countrySierra_Leone",
                                               "Sierra Leone"
                                           },
                                       new[]
                                           {
                                               "countrySingapore",
                                               "Singapore"
                                           },
                                       new[]
                                           {"countrySlovakia", "Slovakia"}
                                       ,
                                       new[]
                                           {"countrySlovenia", "Slovenia"}
                                       ,
                                       new[]
                                           {
                                               "countrySolomon_Islands",
                                               "Solomon Islands"
                                           },
                                       new[] {"countrySomalia", "Somalia"}
                                       ,
                                       new[]
                                           {
                                               "countrySouth_Africa",
                                               "South Africa"
                                           },
                                       new[]
                                           {
                                               "countrySouth_Georgia",
                                               "South Georgia"
                                           },
                                       new[] {"countrySpain", "Spain"},
                                       new[]
                                           {
                                               "countrySri_Lanka",
                                               "Sri Lanka"
                                           },
                                       new[] {"countrySurinam", "Surinam"}
                                       ,
                                       new[]
                                           {
                                               "countrySvalbard_and_Jan_Mayen_Islands"
                                               ,
                                               "Svalbard and Jan Mayen Islands"
                                           },
                                       new[]
                                           {
                                               "countrySwaziland",
                                               "Swaziland"
                                           },
                                       new[] {"countrySweden", "Sweden"},
                                       new[]
                                           {
                                               "countrySwitzerland",
                                               "Switzerland"
                                           },
                                       new[] {"countryTaiwan", "Taiwan"},
                                       new[]
                                           {
                                               "countryTajikistan",
                                               "Tajikistan"
                                           },
                                       new[]
                                           {
                                               "countryTanzania",
                                               "Tanzania, United Republic of"
                                           },
                                       new[]
                                           {"countryThailand", "Thailand"}
                                       ,
                                       new[] {"countryTogo", "Togo"},
                                       new[] {"countryTokelau", "Tokelau"}
                                       ,
                                       new[] {"countryTonga", "Tonga"},
                                       new[]
                                           {
                                               "countryTrinidad_and_Tobago",
                                               "Trinidad and Tobago"
                                           },
                                       new[] {"countryTunisia", "Tunisia"}
                                       ,
                                       new[] {"countryTurkey", "Turkey"},
                                       new[]
                                           {
                                               "countryTurkmenistan",
                                               "Turkmenistan"
                                           },
                                       new[]
                                           {
                                               "countryTurks_and_Caicos_Islands"
                                               ,
                                               "Turks and Caicos Islands"
                                           },
                                       new[] {"countryTuvalu", "Tuvalu"},
                                       new[] {"countryUganda", "Uganda"},
                                       new[] {"countryUkraine", "Ukraine"}
                                       ,
                                       new[]
                                           {
                                               "countryUnited_Arab_Emirates",
                                               "United Arab Emirates"
                                           },
                                       new[]
                                           {
                                               "countryUnited_Kingdom",
                                               "United Kingdom"
                                           },
                                       new[]
                                           {
                                               "countryUnited_States",
                                               "United States"
                                           },
                                       new[]
                                           {
                                               "countryUnited_States_Minor_Outlying_Islands"
                                               ,
                                               "United States Minor Outlying Islands"
                                           },
                                       new[]
                                           {
                                               "countryUnited_States_Virgin_Islands"
                                               ,
                                               "United States Virgin Islands"
                                           },
                                       new[] {"countryUruguay", "Uruguay"}
                                       ,
                                       new[]
                                           {
                                               "countryUzbekistan",
                                               "Uzbekistan"
                                           },
                                       new[] {"countryVanuatu", "Vanuatu"}
                                       ,
                                       new[]
                                           {
                                               "countryVenezuela",
                                               "Venezuela"
                                           },
                                       new[]
                                           {"countryViet_Nam", "Viet Nam"}
                                       ,
                                       new[]
                                           {
                                               "countryWallis_and_Futuna_Islands"
                                               ,
                                               "Wallis and Futuna Islands"
                                           },
                                       new[]
                                           {
                                               "countryWestern_Sahara",
                                               "Western Sahara"
                                           },
                                       new[] {"countryYemen", "Yemen"},
                                       new[] {"countryZambia", "Zambia"},
                                       new[]
                                           {"countryZimbabwe", "Zimbabwe"}
                                       ,
                                       // Time Units
      
                                       new[]
                                           {
                                               "tuMillisecond", "Millisecond"
                                           },
                                       new[]
                                           {
                                               "tuMilliseconds",
                                               "Milliseconds"
                                           },
                                       new[] {"tuSecond", "Second"},
                                       new[] {"tuSeconds", "Seconds"},
                                       new[] {"tuMinute", "Minute"},
                                       new[] {"tuMinutes", "Minutes"},
                                       new[] {"tuHour", "Hour"},
                                       new[] {"tuHours", "Hours"},
                                       new[] {"tuDay", "Day"},
                                       new[] {"tuDays", "Days"},
                                       new[] {"tuWeek", "Week"},
                                       new[] {"tuWeeks", "Weeks"},
                                       new[] {"tuMonth", "Month"},
                                       new[] {"tuMonths", "Months"},
                                       new[] {"tuQuarter", "Quarter"},
                                       new[] {"tuQuarters", "Quarters"},
                                       new[] {"tuYear", "Year"},
                                       new[] {"tuYears", "Years"},
                                       // Expressions
      
                                       new[]
                                           {
                                               "exprEnvNotDefined",
                                               "Environment is not defined"
                                           },
                                       new[]
                                           {
                                               "exprNoResolverForSchema",
                                               "No resolver defined for schema: "
                                           },
                                       new[]
                                           {
                                               "exprErrResolvingReference"
                                               ,
                                               "Error resolving reference ''{0}'': "
                                           },
                                       new[]
                                           {
                                               "exprParseErr",
                                               "Error parsing expression ''{0}'': "
                                           },
                                       // Data tables
      
                                       new[]
                                           {
                                               "dtErrPassingTable",
                                               "Error passing data table through a filter: "
                                           },
                                       new[]
                                           {
                                               "dtNullsNotPermitted",
                                               "Null values are not permitted in field ''{0}''"
                                           },
                                       new[]
                                           {
                                               "dtIllegalFieldValue",
                                               "Illegal value ''{0}'' for field ''{1}'': "
                                           },
                                       new[]
                                           {
                                               "dtFieldNotFound",
                                               "Field ''{0}'' not found in data record"
                                           },
                                       new[]
                                           {
                                               "dtValueNotInSelVals",
                                               "Value is not listed in selection values: "
                                           },
                                       new[]
                                           {
                                               "dtEditorNotSuitable",
                                               "Editor is not suitable for the field: "
                                           },
                                       new[]
                                           {
                                               "dtValueNotComparable",
                                               "Value not comparable"
                                           },
                                       new[]
                                           {
                                               "dtValueTooSmall",
                                               "Value too small (current:new[] {0}, min:new[] {1})"
                                           },
                                       new[]
                                           {
                                               "dtValueTooBig",
                                               "Value too big (current:new[] {0}, max:new[] {1})"
                                           },
                                       new[]
                                           {
                                               "dtValueTooShort",
                                               "Value too short (current:new[] {0}, min:new[] {1})"
                                           },
                                       new[]
                                           {
                                               "dtValueTooLong",
                                               "Value too long (current:new[] {0}, max:new[] {1})"
                                           },
                                       new[]
                                           {
                                               "dtValueDoesNotMatchPattern"
                                               ,
                                               "Value ''{0}'' does''t match to pattern ''{1}''"
                                           },
                                       new[]
                                           {
                                               "dtKeyFieldViolation",
                                               "Key fields violation:new[] {0}"
                                           },
                                       new[]
                                           {
                                               "dtInvalidName",
                                               "Name may contain only letters, digits, and underscores"
                                           }
                                       ,
                                       new[]
                                           {
                                               "dtInvalidDescr",
                                               "Description may contain only printable characters"
                                           },
                                       new[]
                                           {
                                               "dtInvalidIp",
                                               "Invalid IP address"
                                           },
                                       new[]
                                           {
                                               "dtErrCopyingField",
                                               "Error copying field ''{0}''"
                                           }
                                       ,
                                       new[]
                                           {
                                               "dtTargetTableMinRecordsReached"
                                               ,
                                               "Minimum number of records reached in target data table"
                                           }
                                       ,
                                       new[]
                                           {
                                               "dtTargetTableMaxRecordsReached"
                                               ,
                                               "Maximum number of records reached in target data table"
                                           }
                                       ,
                                       new[]
                                           {
                                               "dtEditorContextMask",
                                               "Context Mask"
                                           },
                                       new[]
                                           {
                                               "dtEditorTextArea",
                                               "Text Area"
                                           },
                                       new[]
                                           {
                                               "dtEditorTextEditor",
                                               "Text Editor"
                                           },
                                       new[]
                                           {
                                               "dtEditorTextData",
                                               "Text Data"
                                           },
                                       new[] {"dtInteger", "Integer"},
                                       new[] {"dtString", "String"},
                                       new[] {"dtBoolean", "Boolean"},
                                       new[] {"dtLong", "Long"},
                                       new[] {"dtFloat", "Float"},
                                       new[] {"dtDouble", "Double"},
                                       new[] {"dtDataTable", "Data Table"}
                                       ,
                                       new[] {"dtObject", "Object"},
                                       new[] {"dtDataBlock", "Data Block"}
                                       ,
                                       new[] {"dtReadOnly", "Read-only"},
                                       new[] {"dtNullable", "Nullable"},
                                       new[] {"dtKeyField", "Key"},
                                       new[]
                                           {
                                               "dtSelectionValues",
                                               "Selection Values"
                                           },
                                       new[]
                                           {
                                               "dtExtendableSelVals",
                                               "Extendable Selection Values"
                                           }
                                       ,
                                       new[]
                                           {
                                               "dtEditorViewer",
                                               "Editor/Viewer"
                                           },
                                       // Clients
                                       new[] {"clStateDocked", "Docked"},
                                       new[] {"clStateFloating", "Floating"},
                                       new[] {"clStateSideBarIcon", "Side Bar Icon"},
                                       new[] {"clStateSideBar", "Side Bar"},
                                       new[] {"clSideLeft", "Left"},
                                       new[] {"clSideRight", "Right"},
                                       new[] {"clSideTop", "Top"},
                                       new[] {"clSideBottom", "Bottom"},
                                       new[] {"clServerTime", "Server Time"},

                                             
                                       new[]{ "clInvalidOpcode", "Invalid operation code: " },
                                       new[]{ "clUnknownOpcode", "Unknown operation code: " },
                                       new[]{ "clInvalidMsgCode", "Invalid message code: " },
                                       new[]{ "clUnknownMsgCode", "Unknown message code: " },
                                       new[]{ "clInvalidCmdCode", "Invalid command code: " },
                                       new[]{ "clUnknownCmdCode", "Unknown command code: " },
                                       new[]{ "clErrorHandlingEvent", "Error handling event ''{0}'': " },

                                       
                                       // Plugins
                                       new[] {"pluginsIntegrityCheckFailed", "Plugins integrity check failed"},
                                       new[] {"pluginsErrGettingPlugin", "Error getting plug-in "},
                                       new[] {"pluginsErrStartingSubsystem", "Error starting plugins subsystem: "},
                                       new[] {"pluginsErrLoading", "Error loading plugins: "},
                                       new[] {"pluginsCantGetExisting", "Cannot get existing plugin "},
                                       // Security and permissions
                                       new[]
                                           {
                                               "secNoPerms", "No Permissions"
                                           },
                                       new[] {"secUserPerms", "User"},
                                       new[]
                                           {
                                               "secAdminPerms",
                                               "Administrator"
                                           },
                                       // Devices
      
                                       new[]
                                           {
                                               "devUncompatibleVersion",
                                               "Version of communications protocol used by remote server is not compatible with this version of client"
                                           },
                                       new[]
                                           {
                                               "devAccessDeniedReply",
                                               "Access denied (''{0}'') reply received to ''{1}''"
                                           },
                                       new[]
                                           {
                                               "devServerReturnedError",
                                               "Remote error"
                                           },
                                       new[] {"devUserColon", "user:"},
                                       new[]
                                           {
                                               "devErrConnecting",
                                               "Error connecting tonew[] {0}: "
                                           },
                                       new[]
                                           {
                                               "devVersionMismatch",
                                               "Version mismatch. Server version:new[] {0}, client version :new[] {1}"
                                           },
                                       // Event Filters
      
                                       new[]
                                           {
                                               "efFilterExpr",
                                               "Filter Expression"
                                           },
                                       new[]
                                           {
                                               "efExprIsParameterized",
                                               "Indicates that filter expression is parameterized and may require additional data input upon filter activation"
                                           },
                                       new[]
                                           {
                                               "efHighlightingColor",
                                               "Highlighting Color"
                                           },
                                       new[]
                                           {
                                               "efLimitTimeFrame",
                                               "Limit Time Frame"
                                           },
                                       new[]
                                           {
                                               "efTimeFrame",
                                               "Time Frame (in Time Units)"
                                           },
                                       new[]
                                           {
                                               "efUseCustomEndTime",
                                               "Use Custom End Time"
                                           },
                                       new[] {"efEndTime", "End Time"},
                                       new[]
                                           {"efEventCount", "Event Count"}
                                       ,
                                       new[]
                                           {"efFilterName", "Filter Name"}
                                       ,
                                       new[]
                                           {
                                               "efFilterDesc",
                                               "Filter Description"
                                           },
                                       new[]
                                           {
                                               "efShowDataFieldNames",
                                               "Show field names in event data"
                                           }
                                       ,
                                       new[]
                                           {
                                               "efShowServerContextNames",
                                               "Show context paths along with their descriptions"
                                           },
                                       new[]
                                           {
                                               "efShowServerEventNames",
                                               "Show event names along with their descriptions"
                                           },
                                       new[] {"efEventName", "Event Name"}
                                       ,
                                       new[]
                                           {
                                               "efContextName",
                                               "Context Name"
                                           },
                                       new[]
                                           {"efEventLevel", "Event Level"}
                                       ,
                                       new[] {"efEventData", "Event Data"}
                                       ,
                                       new[]
                                           {
                                               "efEventSourceContext",
                                               "Event Source Context"
                                           },
                                       new[]
                                           {
                                               "efEventTimestamp",
                                               "Event Timestamp"
                                           },
                                       new[]
                                           {
                                               "efAckCount",
                                               "Acknowledgement Count"
                                           },
                                       // Even Log
      
                                       new[]
                                           {
                                               "elEventHistory",
                                               "Event History"
                                           },
                                       new[]
                                           {
                                               "elCurrentEvents",
                                               "Current Events"
                                           },
                                       new[]
                                           {
                                               "elClearTooltip",
                                               "Clear Event List"
                                           },
                                       new[] {"elRefresh", "Refresh"},
                                       new[] {"elFirst", "First"},
                                       new[] {"elLast", "Last"},
                                       new[] {"elPrevious", "Previous"},
                                       new[] {"elNext", "Next"},
                                       new[]
                                           {
                                               "elReactivateFilter",
                                               "Reactivate Current Filter"
                                           },
                                       new[]
                                           {
                                               "elConfigureFilter",
                                               "Configure Current Filter"
                                           },
                                       new[]
                                           {
                                               "elHistoryNotLoaded",
                                               "History not loaded"
                                           },
                                       new[]
                                           {
                                               "elViewSeverityStatistics",
                                               "View Severity Statistics"
                                           }
                                       ,
                                       new[]
                                           {
                                               "elEventSeverityStatistics",
                                               "Event Severity Statistics"
                                           }
                                       ,
                                       new[]
                                           {
                                               "elExportEvents",
                                               "Export Events"
                                           },
                                       new[] {"elRows", "Rows: "},
                                       new[]
                                           {
                                               "elRowsPerPage",
                                               "Rows Per Page"
                                           },
                                       new[] {"elEventData", "Event Data"}
                                       ,
                                       new[]
                                           {
                                               "elShowingOf",
                                               "Showingnew[] {0} -new[] {1} ofnew[] {2}"
                                           }
                                       ,
                                       new[]
                                           {
                                               "elShowEventData",
                                               "Show Event Data"
                                           },
                                       new[]
                                           {
                                               "elEnterAckText",
                                               "Enter acknowledgement text: "
                                           },
                                       new[]
                                           {
                                               "elEvtAck",
                                               "Event Acknowledgement"
                                           },
                                       new[]
                                           {
                                               "elViewAcknowledgements",
                                               "View Acknowledgements"
                                           },
                                       new[]
                                           {
                                               "elAckEvent",
                                               "Acknowledge Event"
                                           },
                                       new[]
                                           {
                                               "elDeleteEvent",
                                               "Delete Event"
                                           },
                                       new[]
                                           {
                                               "elTmDateTime",
                                               "Server Timestamp"
                                           },
                                       new[]
                                           {"elTmAck", "Acknowledgements"}
                                       ,
                                       new[]
                                           {
                                               "elContextActionsMenu",
                                               "Related Actions"
                                           },
                                       new[]
                                           {
                                               "elFilterParams",
                                               "Filter Parameters"
                                           },
                                       new[]
                                           {
                                               "elErrLoadingHistory",
                                               "Error loading event history"
                                           }
                                       ,
                                       new[]
                                           {
                                               "elErrActivatingFilter",
                                               "Error activating filter"
                                           },
                                       new[]
                                           {
                                               "elErrSettingAck",
                                               "Error setting acknowledgement"
                                           },
                                       new[] {"elEventLog", "Event Log"},
                                       // Parameterizer
      
                                       new[]
                                           {
                                               "parameterizerParameterized",
                                               "Parameterized"
                                           },
                                       new[]
                                           {
                                               "parameterizerSource",
                                               "Parameterizer Source Data"
                                           },
                                       // Import/Export
      
                                       new[]
                                           {
                                               "ieFieldDelimiter",
                                               "Field Delimiter"
                                           },
                                       new[]
                                           {
                                               "ieUseTextQualifier",
                                               "Use Text Qualifier"
                                           },
                                       new[]
                                           {
                                               "ieTextQualifier",
                                               "Text Qualifier"
                                           },
                                       new[]
                                           {
                                               "ieCsvExportOptions",
                                               "CSV Export Options"
                                           },
                                       new[]
                                           {
                                               "ieCsvImportOptions",
                                               "CSV Import Options"
                                           },
                                       new[] {"ieDoNotUse", "Do not use"},
                                       new[]
                                           {
                                               "ieUseWhenNecessary",
                                               "Use when necessary"
                                           },
                                       new[] {"ieUseAlways", "Use always"}
                                       ,
                                       new[]
                                           {
                                               "ieCommentChar",
                                               "Comment Character"
                                           },
                                       new[]
                                           {"ieEscapeMode", "Escape Mode"}
                                       ,
                                       new[]
                                           {
                                               "ieEscBackslash",
                                               "Use a backslash character before the text qualifier to represent an occurrence of the text qualifier"
                                           },
                                       new[]
                                           {
                                               "ieEscDouble",
                                               "Double up the text qualifier to represent an occurrence of the text qualifier"
                                           },
                                       new[]
                                           {
                                               "ieHeaderRecord",
                                               "Header Record"
                                           },
                                       new[]
                                           {"ieFieldNames", "Field Names"}
                                       ,
                                       new[]
                                           {
                                               "ieFieldDescriptions",
                                               "Field Descriptions"
                                           },
                                       new[]
                                           {
                                               "ieSkip",
                                               "Ignore As Non-Valuable Data"
                                           }
                                       ,
                                       new[]
                                           {
                                               "ieSelectFields",
                                               "Select Fields"
                                           },
                                       new[]
                                           {
                                               "ieNoDataToProceed",
                                               "No data to proceed"
                                           },
                                       new[]
                                           {
                                               "ieImportedData",
                                               "Imported Data"
                                           },
                                       new[]
                                           {
                                               "ieScriptImportDataHelp",
                                               "This data will be passed as input to the import script."
                                           },
                                       // Scripts
      
                                       new[]
                                           {
                                               "scrScriptCompilationFailed"
                                               ,
                                               "Script compilation failed: "
                                           },
                                       // Widgets
      
                                       new[] {"wComponent", "Component"},
                                       new[]
                                           {
                                               "wCustomFont",
                                               "Use Custom Font"
                                           },
                                       new[] {"wSize", "Size"},
                                       new[] {"wBold", "Bold"},
                                       new[] {"wItalic", "Italic"},
                                       new[] {"wPosition", "Position"},
                                       new[] {"wLine", "Line"},
                                       new[] {"wLowered", "Lowered"},
                                       new[] {"wRaised", "Raised"},
                                       new[] {"wEtched", "Etched"},
                                       new[] {"wTop", "Top"},
                                       new[]
                                           {
                                               "wTopOverlap",
                                               "Top, Overlap Image"
                                           },
                                       new[] {"wLeft", "Left"},
                                       new[] {"wBottom", "Bottom"},
                                       new[]
                                           {
                                               "wBottomOverlap",
                                               "Bottom, Overlap Image"
                                           },
                                       new[] {"wRight", "Right"},
                                       new[]
                                           {
                                               "wPrimaryColor",
                                               "Primary Color"
                                           },
                                       new[]
                                           {
                                               "wSecondaryColor",
                                               "Secondary Color"
                                           },
                                       new[]
                                           {"wTitleColor", "Title Color"},
                                       new[]
                                           {
                                               "wTitleJustification",
                                               "Title Justification"
                                           },
                                       new[] {"wInner", "Inner"},
                                       new[] {"wOuter", "Outer"},
                                       new[]
                                           {"wWeightY", "Vertical Weight"}
                                       ,
                                       new[]
                                           {
                                               "wWeightX",
                                               "Horizontal Weight"
                                           },
                                       new[] {"wFill", "Fill"},
                                       new[]
                                           {
                                               "wRightMargin", "Right Margin"
                                           },
                                       new[]
                                           {"wLeftMargin", "Left Margin"},
                                       new[]
                                           {
                                               "wBottomMargin",
                                               "Bottom Margin"
                                           },
                                       new[] {"wTopMargin", "Top Margin"},
                                       new[]
                                           {"wColumnSpan", "Column Span"},
                                       new[] {"wRowSpan", "Row Span"},
                                       new[] {"wHeightDescr", "Height"},
                                       new[] {"wWidthDescr", "Width"},
                                       new[] {"wGridxDescr", "Column"},
                                       new[] {"wGridyDescr", "Row"},
                                       new[] {"wTargetDescr", "Target"},
                                       new[]
                                           {
                                               "wActivatorDescr", "Activator"
                                           },
                                       new[] {"wOnStartup", "On Startup"},
                                       new[] {"wOnEvent", "On Event"},
                                       new[]
                                           {"wEvaluationPeriod", "Period"}
                                       ,
                                       new[]
                                           {
                                               "wPeriodically",
                                               "Periodically"
                                           },
                                       new[]
                                           {"wBindingsDescr", "Bindings"},
                                       new[] {"wAnchor", "Anchor"},
                                       new[] {"wAnchorPageStart", "North"}
                                       ,
                                       new[] {"wAnchorLineEnd", "East"},
                                       new[] {"wAnchorPageEnd", "South"},
                                       new[] {"wAnchorLineStart", "West"},
                                       new[]
                                           {
                                               "wAnchorFirstLineStart",
                                               "North-West"
                                           },
                                       new[]
                                           {
                                               "wAnchorFirstLineEnd",
                                               "North-East"
                                           },
                                       new[]
                                           {
                                               "wAnchorLastLineEnd",
                                               "South-East"
                                           },
                                       new[]
                                           {
                                               "wAnchorLastLineStart",
                                               "South-West"
                                           },
                                       new[] {"wHorizontal", "Horizontal"}
                                       ,
                                       new[] {"wBoth", "Both"},
                                       new[]
                                           {
                                               "wNameSelectedButton",
                                               "Name of Selected Button"
                                           },
                                       new[] {"wFormat", "Format"},
                                       new[]
                                           {
                                               "wSelectedValue",
                                               "Selected Value"
                                           },
                                       new[]
                                           {
                                               "wMaxRowCount",
                                               "Maximum Displayed Row Count"
                                           }
                                       ,
                                       new[]
                                           {
                                               "wOptionDescription",
                                               "Description"
                                           },
                                       new[]
                                           {
                                               "wSelectionValues",
                                               "Selection Values"
                                           },
                                       new[] {"wFont", "Font"},
                                       new[] {"wTooltip", "Tooltip"},
                                       new[] {"wForeground", "Foreground"}
                                       ,
                                       new[] {"wBackground", "Background"}
                                       ,
                                       new[] {"wOpaque", "Opaque"},
                                       new[] {"wBorder", "Border"},
                                       new[] {"wText", "Text"},
                                       new[] {"wEditable", "Editable"},
                                       new[] {"wCenter", "Center"},
                                       new[] {"wAlignment", "Alignment"},
                                       new[]
                                           {
                                               "wVerticalAlignment",
                                               "Vertical Alignment"
                                           },
                                       new[]
                                           {
                                               "wHorizontalAlignment",
                                               "Horizontal Alignment"
                                           },
                                       new[] {"wActions", "Actions"},
                                       new[] {"wSubmit", "Submit"},
                                       new[] {"wReset", "Reset"},
                                       new[] {"wSelected", "Selected"},
                                       new[]
                                           {
                                               "wButtonGroupID",
                                               "Button Group"
                                           },
                                       new[]
                                           {
                                               "wIncludeHistory",
                                               "Include Historical Data"
                                           },
                                       new[]
                                           {
                                               "wIncludeRealtime",
                                               "Include Real-Time Data"
                                           },
                                       new[] {"wPie3D", "Pie 3D"},
                                       new[] {"wPie", "Pie"},
                                       new[] {"wBar", "Bar"},
                                       new[] {"wRing", "Ring"},
                                       new[] {"wBar3D", "Bar 3D"},
                                       new[]
                                           {"wStackedBar", "Stacked Bar"},
                                       new[]
                                           {
                                               "wStackedBar3D",
                                               "Stacked Bar 3D"
                                           },
                                       new[] {"wChartType", "Chart Type"},
                                       new[]
                                           {
                                               "wLimitTimeRange",
                                               "Limit Time Range"
                                           },
                                       new[] {"wTimeUnit", "Time Unit"},
                                       new[]
                                           {"wDataSource", "Data Source"},
                                       new[]
                                           {
                                               "wValueExpressions",
                                               "Value Expressions"
                                           },
                                       new[] {"wVariable", "Variable"},
                                       new[]
                                           {
                                               "wSourceDataExpression",
                                               "Source Data Expression"
                                           },
                                       new[] {"wStep", "Step"},
                                       new[]
                                           {
                                               "wCategoryExpression",
                                               "Category Expression"
                                           },
                                       new[]
                                           {
                                               "wTimeRange",
                                               "Time Range (in Time Units)"
                                           },
                                       new[] {"wLine3D", "Line 3D"},
                                       new[]
                                           {"wChartTitle", "Chart Title"},
                                       new[] {"wSpline", "Spline"},
                                       new[] {"wRenderer", "Renderer"},
                                       new[]
                                           {
                                               "wVerticalAxisLabel",
                                               "Vertical Axis Label"
                                           },
                                       new[]
                                           {
                                               "wHorizontalAxisLabel",
                                               "Horizontal Axis Label"
                                           },
                                       new[]
                                           {"wOrientation", "Orientation"}
                                       ,
                                       new[]
                                           {"wShowLegend", "Show Legend"},
                                       new[]
                                           {
                                               "wShowTooltips",
                                               "Show Tooltips"
                                           },
                                       new[]
                                           {
                                               "wHorizontalAxisOffset",
                                               "Horizontal Axis Offset"
                                           },
                                       new[]
                                           {
                                               "wVerticalAxisOffset",
                                               "Vertical Axis Offset"
                                           },
                                       new[]
                                           {
                                               "wDrawHorizontalGridlines",
                                               "Draw Horizontal Gridlines"
                                           },
                                       new[]
                                           {
                                               "wDrawVerticalGridlines",
                                               "Draw Vertical Gridlines"
                                           },
                                       new[]
                                           {
                                               "wDrawValueMarks",
                                               "Draw Value Marks"
                                           },
                                       new[] {"wPaint", "Paint"},
                                       new[]
                                           {
                                               "wDataSeriesPaints",
                                               "Data Series Paints"
                                           },
                                       new[] {"wVertical", "Vertical"},
                                       new[] {"wEventData", "Event Data"},
                                       new[]
                                           {
                                               "wVariableData",
                                               "Variable Data"
                                           },
                                       new[]
                                           {"wCustomData", "Custom Data"},
                                       new[]
                                           {
                                               "wLoadingChartData",
                                               "Loading chart data..."
                                           },
                                       new[] {"wScrolling", "Scrolling"},
                                       new[]
                                           {
                                               "wScrollingAuto",
                                               "Smart Scrollbar Policy"
                                           },
                                       new[] {"wDataTable", "Data Table"},
                                       new[]
                                           {
                                               "wOrderNumber", "Order Number"
                                           },
                                       new[] {"wNoImage", "No Image"},
                                       new[]
                                           {
                                               "wImageExpression",
                                               "Image Expression"
                                           },
                                       new[] {"wImageID", "Image ID"},
                                       new[]
                                           {"wImageTable", "Image Table"},
                                       new[] {"wImageData", "Image Data"},
                                       new[] {"wSVGFile", "SVG File"},
                                       new[] {"wDepth", "Depth"},
                                       new[]
                                           {
                                               "wVisibleRowCount",
                                               "Visible Row Count"
                                           },
                                       new[] {"wAuto", "Auto"},
                                       new[]
                                           {
                                               "wLayoutOrientation",
                                               "Layout Orientation"
                                           },
                                       new[]
                                           {
                                               "wSelectionMode",
                                               "Selection Mode"
                                           },
                                       new[]
                                           {
                                               "wSelectedItem",
                                               "Selected Item"
                                           },
                                       new[]
                                           {
                                               "wSelectedItems",
                                               "Selected Items"
                                           },
                                       new[]
                                           {
                                               "wItemDescription",
                                               "Item Description"
                                           },
                                       new[] {"wItemValue", "Item Value"},
                                       new[] {"wListItems", "List Items"},
                                       new[]
                                           {
                                               "wSingleSelection",
                                               "Single Item Selection"
                                           },
                                       new[]
                                           {
                                               "wSingleIntervalSelection",
                                               "Single Interval Selection"
                                           },
                                       new[]
                                           {
                                               "wMultipleIntervalSelection"
                                               ,
                                               "Multiple Intervals Selection"
                                           },
                                       new[]
                                           {
                                               "wHorizontalWrap",
                                               "Horizontal Wrap"
                                           },
                                       new[]
                                           {
                                               "wVerticalWrap",
                                               "Vertical Wrap"
                                           },
                                       new[] {"wMinimum", "Minimum"},
                                       new[] {"wMaximum", "Maximum"},
                                       new[] {"wIcon", "Icon"},
                                       new[]
                                           {
                                               "wMinorTickSpacing",
                                               "Minor Tick Spacing"
                                           },
                                       new[]
                                           {
                                               "wMajorTickSpacing",
                                               "Major Tick Spacing"
                                           },
                                       new[]
                                           {"wPaintTrack", "Paint Track"},
                                       new[]
                                           {"wPaintTicks", "Paint Ticks"},
                                       new[]
                                           {
                                               "wPaintLabels", "Paint Labels"
                                           },
                                       new[]
                                           {
                                               "wCustomLabels",
                                               "Custom Labels"
                                           },
                                       new[]
                                           {
                                               "wDividerSize", "Divider Size"
                                           },
                                       new[]
                                           {
                                               "wResizeWeight",
                                               "Resize Weight"
                                           },
                                       new[]
                                           {
                                               "wDividerLocation",
                                               "Divider Location"
                                           },
                                       new[] {"wActiveTab", "Active Tab"},
                                       new[]
                                           {
                                               "wIgnoreBindingErrors",
                                               "Ignore Binding Errors"
                                           },
                                       new[]
                                           {
                                               "wAppWidth",
                                               "Application Window Width"
                                           },
                                       new[]
                                           {
                                               "wAppHeight",
                                               "Application Window Height"
                                           },
                                       new[]
                                           {
                                               "wAllBindings", "All Bindings"
                                           },
                                       new[] {"wRoot", "Root"},
                                       new[] {"wLineWrap", "Line Wrap"},
                                       new[]
                                           {
                                               "wWrapStyleWord",
                                               "Wrap on Word Bounds"
                                           },
                                       new[] {"wLabel", "Label"},
                                       new[]
                                           {
                                               "wIncorrectResName",
                                               "Incorrect resource name"
                                           },
                                       new[] {"wLayout", "Layout"},
                                       new[]
                                           {"wGridLayout", "Grid Layout"},
                                       new[]
                                           {
                                               "wAbsoluteLayout",
                                               "Absolute Layout"
                                           },
                                       new[]
                                           {
                                               "wXCoordinate", "X Coordinate"
                                           },
                                       new[]
                                           {
                                               "wYCoordinate", "Y Coordinate"
                                           },
                                       new[] {"wZOrder", "Z-Order"},
                                       new[] {"wVisible", "Visible"},
                                       new[]
                                           {
                                               "wNoRootPanelSet",
                                               "No root panel set in provided widget template object"
                                           },
                                       new[] {"wPanel", "Panel"},
                                       new[] {"wSlider", "Slider"},
                                       new[] {"wSpinner", "Spinner"},
                                       new[]
                                           {
                                               "wDataTableEditor",
                                               "Data Table Editor"
                                           },
                                       new[]
                                           {
                                               "wPasswordField",
                                               "Password Field"
                                           },
                                       new[]
                                           {
                                               "wProgressBar", "Progress Bar"
                                           },
                                       new[] {"wImage", "Image"},
                                       new[] {"wDevice", "Device"},
                                       new[] {"wLabels", "Labels"},
                                       new[]
                                           {
                                               "wStatusTable", "Status Table"
                                           },
                                       new[]
                                           {
                                               "wVectorDrawing",
                                               "Vector Drawing"
                                           },
                                       new[] {"wChart", "Chart"},
                                       new[] {"wGauge", "Gauge"},
                                       new[] {"wMeter", "Meter"},
                                       new[] {"wCompass", "Compass"},
                                       new[] {"wList", "List"},
                                       new[] {"wTextField", "Text Field"},
                                       new[] {"wTextArea", "Text Area"},
                                       new[] {"wButton", "Button"},
                                       new[] {"wCheckBox", "Check Box"},
                                       new[] {"wComboBox", "Combo Box"},
                                       new[]
                                           {
                                               "wToggleButton",
                                               "Toggle Button"
                                           },
                                       new[]
                                           {
                                               "wRadioButton", "Radio Button"
                                           },
                                       new[]
                                           {
                                               "wLayeredPanel",
                                               "Layered Panel"
                                           },
                                       new[] {"wLayer", "Layer"},
                                       new[]
                                           {
                                               "wTabbedPanel", "Tabbed Panel"
                                           },
                                       new[] {"wTab", "Tab"},
                                       new[]
                                           {"wSplitPanel", "Split Panel"},
                                       new[] {"wFrame", "Frame"},
                                       new[]
                                           {
                                               "wButtonGroup", "Button Group"
                                           },
                                       new[] {"wScripts", "Scripts"},
                                       new[] {"wCode", "Code"},
                                       new[] {"wMiddle", "Middle"},
                                       new[]
                                           {
                                               "wDeviceImageFilter",
                                               "Image Files (*.jpg, *.jpeg, *.gif, *.png, *.svg)"
                                           },
                                   });
    }
}