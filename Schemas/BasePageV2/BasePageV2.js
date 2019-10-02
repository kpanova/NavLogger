define("BasePageV2", [], function() {
	var FileName;
	var FilePath;
	return {
		methods: {
			init:function () {
				this.callParent(arguments);
				map = new Ext.util.KeyMap(Ext.getBody(), [{
					    key: Ext.EventObject.F7,
					    scope: this,
					    fn: this.onKeyPress
					}]);
				},
			lookupCallback: function(args){
				var fileId = args.selectedRows.collection.items[0].Id;
				var urlPattern = "{0}/rest/FileService/GetFile/{1}/{2}";
				var workspaceBaseUrl = Terrasoft.utils.uri.getConfigurationWebServiceBaseUrl();
				var url = Ext.String.format(urlPattern, workspaceBaseUrl, FilePath, fileId);
				window.open(url);
			},
			onKeyPress: function(arguments){
				this.callService({
						serviceName: "NavLoggerHelper",
						methodName: "GetSysSettingValue",
					},
					function(response) {
						FileName = response.GetSysSettingValueResult.Item2;
						FilePath = response.GetSysSettingValueResult.Item1;

						var config = {
							entitySchemaName: FileName,
							// Множественный выбор отключен.
							multiSelect: false,
							// Отображаемая колонка — [Name].
							columns: ["Name"]
						};
						this.openLookup(config, this.lookupCallback, this);
					}, this);
			}
		},
		diff: /**SCHEMA_DIFF*/[]/**SCHEMA_DIFF*/
	};
});