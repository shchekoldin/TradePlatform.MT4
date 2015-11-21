#property copyright "Copyright TradePlatform, 2005-2013"
#property link      "http://tradeplatform.codeplex.com/"

#include <Core_stdlib.mqh>
#include <Core_winsock.mqh>

#import "user32.dll"
bool GetAsyncKeyState(int Key);
#import

string Bridge_NET_Protocol="TCP";

int conn_socket=0;

int Bridge_NET_Init()
{

}

int Bridge_NET_DeInit()
{
	closesocket(conn_socket);
}

string Bridge_NET_CallFunction(string handlerName, string methodName, string parameters = "")
{
   if(IsTesting())
   {
      return("");
   }
   
	int Buffer[256];
	int retval;
	int i, loopcount=0;
	int addr[1];
	int socket_type;
	int server[sockaddr_in];
	int hp;
	int wsaData[WSADATA];
	
	//closesocket(conn_socket);

	if(handlerName == ""  || methodName == "" || Bridge_NET_IsActive == false)
	{
		return("");
	}

	if (Bridge_NET_Protocol=="TCP")
	{
		socket_type = SOCK_STREAM; 
	}
	else if (Bridge_NET_Protocol=="UDP")
	{
		socket_type = SOCK_DGRAM;
	}

	retval = WSAStartup(0x202, wsaData);
	if (retval != 0) 
	{
		Print("Net Bridge Error: WSAStartup() failed with error "+ retval);
		return("");
	} 
	
	if (isalpha(StringGetChar(Bridge_NET_Server,0)))
	{
		Print("Net Bridge Error: IP address should be given in numeric form in this version.");
		return("");
	} 
	
	addr[0] = inet_addr(Bridge_NET_Server); 
	hp = addr[0];
	
	if (hp == 0 ) 
	{
		Print("Net Bridge Error: Cannot resolve address \""+Bridge_NET_Server+"\": Error :"+WSAGetLastError());
		return("");
	}
	
	int2struct(server,sin_addr,addr[0]);
	int2struct(server,sin_family,AF_INET);
	int2struct(server,sin_port,htons(Bridge_NET_Port));

	conn_socket = socket(AF_INET, socket_type, 0); 
	if (conn_socket <0 ) 
	{
		Print("Net Bridge Error: Error Opening socket: Error "+ WSAGetLastError());
		return("");
	}

	retval=connect(conn_socket, server, ArraySize(server)<<2);
	
	if (retval == SOCKET_ERROR) 
	{
		//	Print("Net Bridge Error: connect() failed: ", WSAGetLastError());
		return("");
	}

	ArrayInitialize(Buffer, 0);
	str2struct(Buffer,ArraySize(Buffer)<<18,Ext_Operator_MagicNumber+"|"+handlerName+"|"+methodName+"|"+parameters+"|");
	retval = send(conn_socket, Buffer, ArraySize(Buffer)<<2, 0);
	
	if (retval == SOCKET_ERROR) 
	{
		Print("Net Bridge Error: send() failed: error ", WSAGetLastError());
		return("");
	}
	ArrayInitialize(Buffer, 0);
	retval = recv(conn_socket, Buffer, ArraySize(Buffer)<<2, 0);
	if (retval == SOCKET_ERROR) 
	{
		Print("Net Bridge Error: recv() failed: error ", WSAGetLastError());
		return("");
	}
	
	if (retval == 0) 
	{
		Print("Net Bridge Error: Server closed connection.");
		return("");
	}
	
	string rawMessage = struct2str(Buffer,ArraySize(Buffer)<<18);
	
	while(StringSubstr(rawMessage, 0, 9) == "###MQL###")
	{
		string message[];
		SplitString2(rawMessage, "|", message);
		
		
		//	string lastError = GetLastError();
		string value = Bridge_NET_MQL(message);
		int errorCode = GetLastError();
		string lastError = ErrorDescription(errorCode);
		ArrayInitialize(Buffer, 0);
		
		if(value == "###NORESULT###")
		{
			lastError = "No mql methond found";
			Print("Net Bridge Error: no mql methond found ");
		}
		
		if(value == "")
		{
			value = "###EMPTY###";
		}
		
		str2struct(Buffer,ArraySize(Buffer)<<18,errorCode+":"+lastError + "|" + value);
		retval = send(conn_socket, Buffer, ArraySize(Buffer)<<2, 0);
		
		if (retval == SOCKET_ERROR)
		{
			Print("Net Bridge Error: send() failed: error ", WSAGetLastError());
			return("");
		} 
		
		ArrayInitialize(Buffer, 0);
		retval = recv(conn_socket, Buffer, ArraySize(Buffer)<<2, 0);
		
		if (retval == SOCKET_ERROR) 
		{
			Print("Client: recv() failed: error ", WSAGetLastError());
			return("");
		} 
		
		
		rawMessage = struct2str(Buffer,ArraySize(Buffer)<<18);
	}
	
	closesocket(conn_socket);
	
	if(StringSubstr(rawMessage, 0, 9) == "###ERR###")
	{
		string message2[];
		SplitString2(rawMessage, "|", message2);
		
		Print("Net Bridge Error: C# Exception: " +  message2[1]);
		return("");
	}

	return(rawMessage);
}

bool SplitString2(string stringValue, string separatorSymbol, string& results[])
{
	//	 Alert("--SplitString--");
	//	 Alert(stringValue);

	if (StringFind(stringValue, separatorSymbol) < 0)
	{// No separators found, the entire string is the result.
		ArrayResize(results, 1);
		results[0] = stringValue;
	}
	else
	{   
		int separatorPos = 0;
		int newSeparatorPos = 0;
		int size = 0;

		while(newSeparatorPos > -1)
		{
			size = size + 1;
			newSeparatorPos = StringFind(stringValue, separatorSymbol, separatorPos);
			
			ArrayResize(results, size);
			if (newSeparatorPos > -1)
			{
				if (newSeparatorPos - separatorPos > 0)
				{  // Evade filling empty positions, since 0 size is considered by the StringSubstr as entire string to the end.
					results[size-1] = StringSubstr(stringValue, separatorPos, newSeparatorPos - separatorPos);
				}
			}
			else
			{  // Reached final element.
				results[size-1] = StringSubstr(stringValue, separatorPos, 0);
			}
			
			
			//Alert(results[size-1]);
			separatorPos = newSeparatorPos + 1;
		}
	}   

	//  if (expectedResultCount == 0 || expectedResultCount == ArraySize(results))
	{  // Results OK.
		return (true);
	}
	//else
	{  // Results are WRONG.
		//     Trace("ERROR - size of parsed string not expected.", true);
		// return (true);
	}
}

bool isalpha(int c) {
   
	string t="0123456789";
	return(StringFind(t,CharToStr(c))<0);

}