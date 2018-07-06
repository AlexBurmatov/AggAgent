using System;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;

namespace WMIAgent
{
    public class ModificationTimeVariable
    {
        private const String V_MODTIME = "modtime";
        private const String VF_MODTIME_MODTIME = "modtime";
        private const String VF_MODTIME_VARIABLE = "variable";

        private static readonly TableFormat VFT_MODTIME = ModificationTimeRecordFormat();

        private static TableFormat ModificationTimeRecordFormat()
        {
            var format = new TableFormat(1, 100);
            format.addField("<" + VF_MODTIME_VARIABLE + "><S><D=Variable Name>");
            format.addField("<" + VF_MODTIME_MODTIME + "><D><D=Modification Time>");
            return format;
        }

        public void addTo(Context context)
        {
            var vd = new VariableDefinition(V_MODTIME, VFT_MODTIME, true, true, "Modification Times", null);
            vd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => tooOldModifications()));
            vd.setSetter(new DelegatedVariableSetter((con, def, caller, request, value) => { }));
            context.addVariableDefinition(vd);
        }

        private static DataTable tooOldModifications()
        {
            var dataTable = new DataTable(ModificationTimeRecordFormat());
            var rec = dataTable.addRecord();
            rec.setValue(VF_MODTIME_VARIABLE, WmiAgent.V_SETTINGS);
            rec.setValue(VF_MODTIME_MODTIME, DateTime.MinValue);
            return dataTable;
        }
    }
}