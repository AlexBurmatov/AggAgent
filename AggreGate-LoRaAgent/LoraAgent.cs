using com.tibbo.aggregate.common.agent;
using com.tibbo.aggregate.common.context;
using com.tibbo.aggregate.common.datatable;
using com.tibbo.aggregate.common.datatable.field;
using com.tibbo.aggregate.common.device;
using com.tibbo.aggregate.common.@event;
using System;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using com.tibbo.aggregate.common.data;

namespace AggreGate_LoRaAgent
{
    class AuthRequest
    {
        public string cmd = "auth_req";
        public string login;
        public string password;
    }

    class AuthResponse
    {
        public string cmd;
        public bool status;
        public string err_string, token, device_access;
        public bool consoleEnable;
        public string[] command_list;
        public RxSettings rx_settings;
    }

    class RxSettings
    {
        public bool unsolicited;
        public string direction;
        public bool withMacCommands;
    }

    class UserListRequest
    {
        public string cmd = "get_users_req";
        public string keyword;
    }

    class UserListResponse
    {
        public string cmd;
        public bool status;
        public string err_string;
        public UserList[] user_list;
    }

    class UserList
    {
        public string login, device_access;
        public bool consoleEnable;
        public string[] devEui_list;
        public string[] command_list;
        public RxSettings rx_settings;
    }

    class DeviceListRequest
    {
        public string cmd = "get_device_appdata_req";
        // + 2 optional fields
    }

    class DeviceListResponse
    {
        public string cmd;
        public bool status;
        public string err_string;
        public DeviceList[] devices_list;
    }

    class DeviceList
    {
        public string devEui, devName, adress1, devType, name;
    }

    class DataDeviceRequest
    {
        public string cmd = "get_data_req";
        public string devEui;
        public SelectForDataDeviceRequest select;
    }

    class SelectForDataDeviceRequest
    {
        public int date_from, date_to, begin_index, limit;
        public string direction;
        public bool withMacCommands;
    }

    class DataDeviceResponse
    {
        public string cmd;
        public bool status;
        public string err_string, devEui, direction;
        public int totalNum;

        public DataList[] data_list;
    }

    class DataList
    {
        public long ts, fcnt, port, freq, rssi;
        public float snr;
        public bool ack;
        public string gatewayId, data, macData, dr, type, packetStatus;
    }

    // 
    class BaseSettings
    {
        bool activation_type;//0-OTAA,1-ABP
        bool request_packet_confirmation;//off,on
        byte period;//1,6,12,24
        bool type_input1;//0-pulsed,1-security
        bool type_input2;//0-pulsed,1-security
        bool type_input3;//0-pulsed,1-security
        bool type_input4;//0-pulsed,1-security
    }

    class DataPacket
    {
        public int type;//1, 2 или 3
        public int charge;//заряд батареи %
        public int type_baseSettings;
        public BaseSettings baseSettings;
        public uint time;
        public DateTime dateTime;
        public int temperature;//-40..+85
        public int input1;
        public int input2;
        public int input3;
        public int input4;

        static public DataPacket Parse(string data)
        {
            DataPacket obj = new DataPacket();
            CultureInfo provider = new CultureInfo("en-US");

            string temp_time = "";
            temp_time += data.Substring(12, 2);
            temp_time += data.Substring(10, 2);
            temp_time += data.Substring(8, 2);
            temp_time += data.Substring(6, 2);

            string temp_input = "";
            temp_input += data.Substring(22, 2);
            temp_input += data.Substring(20, 2);
            temp_input += data.Substring(18, 2);
            temp_input += data.Substring(16, 2);


            int.TryParse(data.Substring(0, 2), NumberStyles.HexNumber, provider, out obj.type);
            int.TryParse(data.Substring(2, 2), NumberStyles.HexNumber, provider, out obj.charge);
            int.TryParse(data.Substring(4, 2), NumberStyles.HexNumber, provider, out obj.type_baseSettings);
            uint.TryParse(temp_time, NumberStyles.HexNumber, provider, out obj.time);
            int.TryParse(data.Substring(14, 2), NumberStyles.HexNumber, provider, out obj.temperature);
            int.TryParse(temp_input, NumberStyles.HexNumber, provider, out obj.input1);
            int.TryParse(data.Substring(24, 8), NumberStyles.HexNumber, provider, out obj.input2);
            int.TryParse(data.Substring(32, 8), NumberStyles.HexNumber, provider, out obj.input3);
            int.TryParse(data.Substring(40, 8), NumberStyles.HexNumber, provider, out obj.input4);

            obj.dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(obj.time);

            return obj;
        }
    }

    internal class LoraAgent
    {
        private const String E_EVENT = "event_test";
        private const String EF_EVENT_DATA = "data";

        private static ClientWebSocket client;
        private String login, password;

        private static FieldFormat ff = FieldFormat.create("data", FieldFormat.STRING_FIELD);
        private static TableFormat singleString = new TableFormat(1, 1, ff);
        private static TableFormat multiString = new TableFormat(1, 1000, ff);
        private static DataTable authData = new DataTable(singleString);
        private static DataTable authData2 = new DataTable(singleString);

        private static FieldFormat ff_login = FieldFormat.create("login", FieldFormat.STRING_FIELD, "Логин");
        private static FieldFormat ff_devacc = FieldFormat.create("device_access", FieldFormat.STRING_FIELD, "Доступ к устройствам");
        private static TableFormat VTF_USERS = new TableFormat(1, 100);
        private static DataTable VDT_USERS = new DataTable(VTF_USERS);

        private static FieldFormat ff_devEui = FieldFormat.create("devEui", FieldFormat.STRING_FIELD, "EUI");
        private static FieldFormat ff_devName = FieldFormat.create("devName", FieldFormat.STRING_FIELD, "Имя устройства");
        private static FieldFormat ff_devType = FieldFormat.create("devType", FieldFormat.STRING_FIELD, "Тип устройства");
        private static FieldFormat ff_devData = FieldFormat.create("devData", FieldFormat.DATATABLE_FIELD, "Данные устройства");
        private static TableFormat VTF_DEVICES = new TableFormat(1, 100);
        private static DataTable VDT_DEVICES = new DataTable(VTF_DEVICES);

        private static FieldFormat ff_dataType = FieldFormat.create("dataType", FieldFormat.INTEGER_FIELD, "Тип пакета");
        private static FieldFormat ff_dataCharge = FieldFormat.create("dataCharge", FieldFormat.INTEGER_FIELD, "Заряд батареи");
        private static FieldFormat ff_dataDateTime = FieldFormat.create("dataDateTime", FieldFormat.DATE_FIELD, "Дата");
        private static FieldFormat ff_dataTemperature = FieldFormat.create("dataTemperature", FieldFormat.INTEGER_FIELD, "Температура");
        private static FieldFormat ff_dataInput1 = FieldFormat.create("dataInput1", FieldFormat.INTEGER_FIELD, "Вход 1");
        private static FieldFormat ff_dataInput2 = FieldFormat.create("dataInput2", FieldFormat.INTEGER_FIELD, "Вход 2");
        private static FieldFormat ff_dataInput3 = FieldFormat.create("dataInput3", FieldFormat.INTEGER_FIELD, "Вход 3");
        private static FieldFormat ff_dataInput4 = FieldFormat.create("dataInput4", FieldFormat.INTEGER_FIELD, "Вход 4");
        private static TableFormat VTF_DATADEVICES = new TableFormat(1, 100);
        private static DataTable VDT_DATADEVICES = new DataTable(VTF_DATADEVICES);

        //private static FieldFormat ff_typeEvent = FieldFormat.create("type", FieldFormat.INTEGER_FIELD, "type");
        //private static FieldFormat ff_dataEvent = FieldFormat.create("data", FieldFormat.FLOAT_FIELD, EF_EVENT_DATA);
        //private static TableFormat EFT_EVENT = new TableFormat(1, 1);
        private static readonly TableFormat EFT_EVENT = new TableFormat(1, 1, "<" + EF_EVENT_DATA + "><F>");

        static LoraAgent()
        {
            VTF_USERS.addField(ff_login);
            VTF_USERS.addField(ff_devacc);
            VTF_DEVICES.addField(ff_devEui);
            VTF_DEVICES.addField(ff_devName);
            VTF_DEVICES.addField(ff_devType);
            VTF_DEVICES.addField(ff_devData);
            VTF_DATADEVICES.addField(ff_dataType);
            VTF_DATADEVICES.addField(ff_dataCharge);
            VTF_DATADEVICES.addField(ff_dataDateTime);
            VTF_DATADEVICES.addField(ff_dataTemperature);
            VTF_DATADEVICES.addField(ff_dataInput1);
            VTF_DATADEVICES.addField(ff_dataInput2);
            VTF_DATADEVICES.addField(ff_dataInput3);
            VTF_DATADEVICES.addField(ff_dataInput4);
            //EFT_EVENT.addField(ff_typeEvent);
            //EFT_EVENT.addField(ff_dataEvent);
        }

        private static async Task<AuthResponse> Connect(string _login, string _password)
        {
            client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://lora.elecom-nt.ru:8002"), new CancellationToken());

            object requestObj = new AuthRequest() { login = _login, password = _password };
            string request = JsonConvert.SerializeObject(requestObj);
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(request));
            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[10024]);
            WebSocketReceiveResult result = await client.ReceiveAsync(bytesReceived, CancellationToken.None);

            var response = JsonConvert.DeserializeObject<AuthResponse>(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
            return response;
        }

        private static async Task<UserListResponse> GetUserList()
        {
            object requestObj = new UserListRequest();
            string request = JsonConvert.SerializeObject(requestObj);
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(request));
            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[10024]);
            WebSocketReceiveResult result = await client.ReceiveAsync(bytesReceived, CancellationToken.None);

            var response = JsonConvert.DeserializeObject<UserListResponse>(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
            return response;
        }

        private static async Task<DeviceListResponse> GetDeviceList()
        {
            object requestObj = new DeviceListRequest();
            string request = JsonConvert.SerializeObject(requestObj);
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(request));
            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[10024]);
            WebSocketReceiveResult result = await client.ReceiveAsync(bytesReceived, CancellationToken.None);

            var response = JsonConvert.DeserializeObject<DeviceListResponse>(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
            return response;
        }

        private static async Task<DataDeviceResponse> GetDataFromDevice(string _devEui)
        {
            object requestObj = new DataDeviceRequest() { devEui = _devEui, select = new SelectForDataDeviceRequest() { limit = 20 } }; // нужно снять ограничения (мб циклом)
            string request = JsonConvert.SerializeObject(requestObj);
            ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(request));
            await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);

            ArraySegment<byte> bytesReceived = new ArraySegment<byte>(new byte[10024]);
            WebSocketReceiveResult result = await client.ReceiveAsync(bytesReceived, CancellationToken.None);

            var response = JsonConvert.DeserializeObject<DataDeviceResponse>(Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count));
            return response;
        }

        public static void Run()
        {
            while (true)
            {
                Thread eventGenerator = null;
                try
                {
                    var rls = new RemoteLinkServer(AggreGateNetworkDevice.DEFAULT_ADDRESS, Agent.DEFAULT_PORT,
                                                   RemoteLinkServer.DEFAULT_USERNAME, RemoteLinkServer.DEFAULT_PASSWORD);

                    var agent = new Agent(rls, "LoraAgent", false);                    
                    initializeAgentContext(agent.getContext());
                    agent.connect();

                    eventGenerator = new Thread(() =>
                    {
                        var random = new Random();
                        while (true)
                        {
                            Thread.Sleep((Int32)3000);
                            if (agent.getContext().isSynchronized())
                            {
                                DataTable records = new DataTable(EFT_EVENT);
                                records.addRecord().addFloat((float)(random.Next() * 1000000));
                                //agent.getContext().fireEvent(E_EVENT, EventLevel.INFO, records/*(float)(random.Next() * 1000000)*/);
                                /*records.addRecord().addFloat((float)random.NextDouble()).addDate(DateTime.Now);
                                agent.getContext().setVariable("setting", records);*/
                            }
                        }
                    })
                    { IsBackground = true };
                    eventGenerator.Start();
                    
                    while (true)
                    {
                        agent.run();
                    }
                }
                catch (Exception ex)
                {
                    if (eventGenerator != null)
                        eventGenerator.Abort();
                    Console.Out.WriteLine(ex.ToString());
                }
            }
        }       

        private static void initializeAgentContext(AgentContext context)
        {
            string login = "arsenyperm";
            string password = "7469494244";            

            VariableDefinition vd = new VariableDefinition("currentUser_login", singleString, true, false, "Имя пользователя", ContextUtils.GROUP_REMOTE);
            vd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => authData));
            vd.setSetter(new DelegatedVariableSetter((con, def, caller, request, value) => { authData = value; }));
            vd.setGroup(ContextUtils.createGroup(ContextUtils.GROUP_REMOTE, "Учетные данные"));
            context.addVariableDefinition(vd);

            vd = new VariableDefinition("currentUser_pswd", singleString, true, false, "Пароль", ContextUtils.GROUP_REMOTE);
            vd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => authData2));
            vd.setSetter(new DelegatedVariableSetter((con, def, caller, request, value) => { authData2 = value; }));
            vd.setGroup(ContextUtils.createGroup(ContextUtils.GROUP_REMOTE, "Учетные данные"));
            context.addVariableDefinition(vd);

            vd = new VariableDefinition("user_list", VTF_USERS, true, false, "Список пользователей", ContextUtils.GROUP_REMOTE);
            vd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => VDT_USERS));
            vd.setSetter(new DelegatedVariableSetter((con, def, caller, request, value) => { VDT_USERS = value; }));
            vd.setGroup(ContextUtils.createGroup(ContextUtils.GROUP_REMOTE, "Данные с сервера"));
            context.addVariableDefinition(vd);

            vd = new VariableDefinition("device_list", VTF_DEVICES, true, false, "Список устройств", ContextUtils.GROUP_REMOTE);
            vd.setGetter(new DelegatedVariableGetter((con, def, caller, request) => VDT_DEVICES));
            vd.setSetter(new DelegatedVariableSetter((con, def, caller, request, value) => { VDT_DEVICES = value; }));
            vd.setGroup(ContextUtils.createGroup(ContextUtils.GROUP_REMOTE, "Данные с сервера"));
            context.addVariableDefinition(vd);
            
            Task<AuthResponse> signin = Connect(login, password);
            signin.Wait();
            if (signin.Result.status)
            {
                var records = new DataTable(singleString);
                records.addRecord().addString(login);
                context.setVariable("currentUser_login", records);

                records = new DataTable(singleString);
                records.addRecord().addString(password);
                context.setVariable("currentUser_pswd", records);
            }

            Task<UserListResponse> getUserList = GetUserList();
            getUserList.Wait();
            if (getUserList.Result.status && getUserList.Result.user_list != null)
            {
                var records = new DataTable(VTF_USERS);
                foreach (var user in getUserList.Result.user_list)
                {
                    records.addRecord().addString(user.login).addString(user.device_access);
                }
                context.setVariable("user_list", records);
            }

            Task<DeviceListResponse> getDevList = GetDeviceList();
            getDevList.Wait();
            var recordsdev = new DataTable(VTF_DEVICES);
            if (getDevList.Result.status && getDevList.Result.devices_list != null)
            {
                foreach (var device in getDevList.Result.devices_list)
                {
                    if (device.devEui == "323632317D37590E")
                    {
                        // запрос архива данных
                        Task<DataDeviceResponse> getDataArchive = GetDataFromDevice(device.devEui);
                        getDataArchive.Wait();
                        var dataDevice = getDataArchive.Result.data_list;
                        var dataTableDevice = new DataTable(VTF_DATADEVICES);
                        foreach (var x in dataDevice)
                        {
                            var packet = DataPacket.Parse(x.data);
                            dataTableDevice.addRecord().addInt(packet.type).addInt(packet.charge).addDate(packet.dateTime).addInt(packet.temperature).addInt(packet.input1).addInt(packet.input2).addInt(packet.input3).addInt(packet.input4);
                        }

                        // добавляем всю информацию
                        recordsdev.addRecord().addString(device.devEui).addString(device.devName).addString(device.devType == null ? " " : device.devType).addDataTable(dataTableDevice);
                    }
                }
                context.setVariable("device_list", recordsdev);
            }

            var ed = new EventDefinition(E_EVENT, EFT_EVENT, "Agent Event", ContextUtils.GROUP_REMOTE);
            context.addEventDefinition(ed);            

            context.addEventListener(E_EVENT, new DefaultContextEventListener<CallerController<CallerData>>(
                (anEvent) =>
                {
                    Console.Out.WriteLine(("User has confirmed event with data: " + anEvent.ToString()));
                }));

            DataTable records2 = new DataTable(EFT_EVENT);
            records2.addRecord().addFloat((float)15);
            context.fireEvent(E_EVENT, EventLevel.INFO, records2/*(float)(random.Next() * 1000000)*/);
            //context.start();
        }
    }
}
