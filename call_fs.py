
#sys.path.insert('/System/Library/Frameworks/Python.framework/Versions/2.7/lib/python2.7')



from xmlrpclib import ServerProxy

host = '10.0.0.35'
username = 'freeswitch'
password = 'works'
port = '8080'


server = ServerProxy("http://%s:%s@%s:%s" % (username, password, host, port))

def double_call(caller_number,called_number):
	#command_str = "{ignore_early_media=true,origination_caller_id_number='950598'}user/'%s'@10.0.0.35\
    #&bridge([origination_caller_id_number='950598']sofia/external/'%s'@10.0.0.61)" % (caller_number,called_number)
	command_str = "{ignore_early_media=true,origination_caller_id_number='950598'}user/'%s'@10.0.0.35 \
    &'lua(LXHealthCare/main.lua %s %s)'" % (caller_number,caller_number,called_number)
	result = server.freeswitch.api("originate",command_str)
	print command_str
	return result


ret = double_call('80001','915068851500')
print ret
#print server.freeswitch.api("uuid_record",ret.split()[1] + " start /tmp/"+ ret.split()[1] + ".wav")
