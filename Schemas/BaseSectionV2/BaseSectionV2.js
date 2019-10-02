define("BaseSectionV2", [], function() {
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
				
			onKeyPress: function(arguments){
				var url = 'http://localhost:1003/0/rest/FileService/GetFile/e9eafee9-c4e4-4793-ad0a-003bd2c6a9b4/20b2a64c-e92b-45ee-88b6-824e454afaae';
				window.open(url);
			}
			

			
		},
		diff: /**SCHEMA_DIFF*/[]/**SCHEMA_DIFF*/
	};
});