#*************************************************
#MCBOT
#Programed by Citidel \  Will Robinson
#Open Code
#Originally made for OTEgamers.com
#*************************************************
#Package Includes
#*************************************************
package require Tcl 8.5
package require eggdrop 1.6.20
package require json 1.1.2
package require http
#*************************************************
# IRC CHANNEL
#*************************************************
set subchannel #OTEgamers
#*************************************************
#Global Variables
#*************************************************
set upthread  www.otegamers.com/topic/6945-
set anntim 0
set annonmsg ""
#*************************************************
#Binds and Commands
#*************************************************
bind join - * join:ote
bind pubm - "*!update" ote:update
bind pubm - "*!update pixelmon" ote:updatepixel
bind pubm - "*!update rr" ote:update
bind pubm - "*!update ftb" ote:updateftb
bind pubm - "*!help" ote:help
bind pubm - "*!help mytown" ote:mytown
bind pubm - "*!help balance" ote:money
bind pubm - "*!help version" ote:version
bind pubm - "*!help shops" ote:shops
bind pubm - "*!help warp" ote:warp
bind pubm - "*!help mining" ote:mining
bind pubm - "*!tps" ote:tps
bind pubm - "*!tps rr" ote:tpsrr
bind pubm - "*!tps ftb" ote:tpsftb
bind pubm - "*!edgebot" ote:commands
bind pubm - "*!edgebot update" ote:read
bind pubm - "*!dj loop" ote:loops
bind EVNT - preinit-server ote:readbot
bind EVNT - init-server ote:readbot
bind pubm - "*!admin announce *" ote:setann
#bind time - * ote:announce 
bind pubm - "*!check *" ote:check
bind pubm - "*!edgebot kick citidel*" ote:jokes
bind pubm - "*!smug" ote:wot
bind pubm - "*!endportal" ote:end
bind pubm - "*!banned *" ote:banned
bind pubm - "*!minecheck" ote:minechk
bind pubm - "*!minestatus" ote:minechk
#*************************************************
#Program Procedures
#each Proc should have a comment detailing how it operates
#*************************************************
proc ote:jokes {nick uhost hand chan text} {
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_"} {
			putserv "PRIVMSG $chan :  $nick, I will not kick him... he keeps me fed... and plays with my buttons"
			}
}
proc ote:wot {nick uhost hand chan text} {
if {[isop $nick $chan] || $nick == "DrSmugleaf" || $nick == "DrSmugleaf_"} {
		if {[expr {int(rand() * 3)} ]< 2} {
			putserv "PRIVMSG $chan :  U WOT!?!"
			} else {
			putserv "PRIVMSG $chan :  HUEHUEHUEHUEHUEHUEHUEHUEHUE!!"
			} 
		} else {
			putserv "PRIVMSG $chan : This command is useless"
		}
if {$nick == "RR1"} {
		if {[string compare -nocase -length 8 $text "DrSmugleaf: !smug"] == 0} {
		if {[expr {int(rand() * 3)} ]< 2} {
			putserv "PRIVMSG $chan :  U WOT!?!"
			} else {
			putserv "PRIVMSG $chan :  HUEHUEHUEHUEHUEHUEHUEHUEHUE!!"
			}
			} 
		}
}
proc ote:loops {nick uhost hand chan text} {
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_"} {
			putserv "PRIVMSG $chan :  This is how to not create a Power Loop: http://puu.sh/5Uv1o"
			}
}
proc ote:banned {nick uhost hand chan text} {
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_"} {
		set otecheck [regsub -all {\!banned } $text ""]
			putserv "PRIVMSG $chan : Guess What $otecheck ? YOU'VE WON! FOLLOW THIS LINK: http://goo.gl/IaXkZl"
			}
}

proc ote:end {nick uhost hand chan text} {
			putserv "PRIVMSG $chan :  $nick the end portal is at: X: -855 Z: -4 Y: 29"

}
proc ote:adminchk { textmsg } {
set ignadmin {Cozza38 Helkarakse Citidel Shotexpert Duf ArbitraryHubris DJDarkStorm Mirokoth F4FRyahn}
 foreach user $ignadmin {
        if {[string compare -nocase -length 5 $textmsg $user] == 0} {
			unset ignadmin
			return 1
		} else {
			unset ignadmin
			return 0
		}
        }
}

#*************************************************
#Fishbans Check, Will check fishbans based off of user Input
#*************************************************
proc ote:check {nick uhost hand chan text} {
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_" || [ote:adminchk $text] == 1} {
		package require json 1.1.2
		set otecheck [regsub -all {\!check } $text ""]
		set apireturn [ote:tpsget "http://callback.api.fishbans.com/stats/$otecheck/otecallback"]
		set output [::json::json2dict $apireturn]
	putserv "NOTICE $nick : Checking Fishbans:[lindex $output 0] = [lindex $output 1]"
	putserv "NOTICE $nick : User Name: [lindex [lindex $output 3] 1]"
	putserv "NOTICE $nick : Total Bans: [lindex [lindex $output 3] 5]"
	putserv "NOTICE $nick : [lindex [lindex [lindex $output 3] 7] 0]: [lindex [lindex [lindex $output 3] 7] 1] || [lindex [lindex [lindex $output 3] 7] 2]: [lindex [lindex [lindex $output 3] 7] 3] || [lindex [lindex [lindex $output 3] 7] 4]: [lindex [lindex [lindex $output 3] 7] 5] || [lindex [lindex [lindex $output 3] 7] 6]: [lindex [lindex [lindex $output 3] 7] 7] || Auth: [lindex [lindex [lindex $output 3] 7] 9]"
	putserv "NOTICE $nick :  http://fishbans.com/u/$otecheck"
	unset otecheck apireturn output
	}
}


#*************************************************
#Bot Connection Update Parser
#Will read and upate data based on MCIRCBOT.cfg on Connect
#*************************************************
proc ote:readbot {ev} {
	global upthread
	set count 0
	set otefile [open /home/eggdrop/eggdrop/scripts/otedata.tcl]
    while {[gets $otefile line] >= 0} {
	  	if {[string match *COMMENT* $line]} {
    		incr count 1
		} else { 
    		if {$count == 2} {
    			# Server Update Threads
    			set upthread $line
    		}
    		}	
    	}
    close otefile
	unset count
}
#*************************************************
#Bot Connection Update Parser
#Will read and upate data based on MCIRCBOT.cfg on Connect
#*************************************************
proc ote:read {nick uhost hand chan text} {
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_"} {
	putquick "NOTICE $nick : reading Data"
	global upthread
	set count 0
	putquick "NOTICE $nick : Opening File"
    set otefile [open /home/eggdrop/eggdrop/scripts/otedata.tcl]
	putquick "NOTICE $nick : File Open $otefile"
    while {[gets $otefile line] >= 0} {
	  	if {[string match *COMMENT* $line]} {
    		incr count 1
			putquick "NOTICE $nick : $count $line"
		} else { 
    		    		if {$count == 2} {
    			# Server Update Threads
    			set upthread $line
    			putquick "NOTICE $nick : Server Update Thread $line"
    			}
    			}			
    	}
		close otefile
		unset count
    }
    	
}

#*************************************************
#Help Command Procedure
#*************************************************
proc ote:commands {nick uhost hand chan text} {
	putquick "NOTICE $nick :$nick, EdgeBot at your service! Commands to use me:"
	putserv "NOTICE $nick : !update  || !help || !tps || !edgebot"
	putserv "NOTICE $nick : !help Sub commands: !help mytown || !help balance || !help version || !help warp || !help mining || !help shops"
	putserv "NOTICE $nick : Future Commands: !innedanadult, !servers, !quote"
}
#*************************************************
#HTTP GET
#Access to HTTP data for TPS and Fishbans
#MUST HAVE TCLLIB INSTALLED!
#*************************************************
proc ote:tpsget { website } {
    package require http
    # send the http request, -timeout sets up a timeout to occur after the specified number of milliseconds
    # we use catch to avoid an abort of the script in case of an error when executing http::geturl (e.g. due to an unsupported url)
    if { [catch { set token [http::geturl $website -timeout 3000]} error] } {
        putcmdlog "ote:tpsget: Error: $error"
        # if the the site does not exist
    } elseif { [http::ncode $token] == "404" } {
        putcmdlog "ote:tpsget: Error: [http::code $token]"
        # check if the request was successful, if yes -> put the html source code into $data
    } elseif { [http::status $token] == "ok" } {
        set data [http::data $token]
        # if a timeout has occurred, send "Timeout occured" to the standad output device
    } elseif { [http::status $token] == "timeout" } {
        putcmdlog "ote:tpsget: Timeout occurred"
        # send the error to the standard output device if there is one
    } elseif { [http::status $token] == "error" } {
        putcmdlog "ote:tpsget: Error: [http::error $token]"
    }
    # last but not least, release the memory which was used for these operations
    http::cleanup $token
    if { [info exists data] } {
	    return $data
	} else {
	    return 0
		
    }
}
#*************************************************
#HTML Helper, strips out HTML tags
#*************************************************
proc ote:striphtml { htmlText } {
    regsub -all {<[^>]+>} $htmlText "" newText
    return $newText 
}
#*************************************************
#*************************************************
proc ote:setann {nick uhost hand chan text} {
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_" || [ote:adminchk $text] == 1} {
		putserv "NOTICE $nick : usage: !admin announce <time> <message> | time should be 10-59 message can be any lenght..."
	global anntim
	global annonmsg
		set oteann [regsub -all {\!admin announce } $text ""]
		set anntim [string trimright [string range $oteann 0 1]]
		set annonmsg [string range $oteann 2 [string length $oteann]]
	}
}
#*************************************************
#Calls the Announce Every interval set with ote:setann 
#*************************************************
proc ote:announce { min hour day month year } {
global anntim
global annonmsg
		if {[scan $min %d] %[expr $anntim*1] == 0} {
			putserv "PRIVMSG #otegamers : $annonmsg"
		}
}

#*************************************************
#TPS Display Command
#*************************************************	
proc ote:tps {nick uhost hand chan text} { 
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_" || [ote:adminchk $text] == 1} {
    set apireturn [ote:tpsget "http://dev.otegamers.com/api/v1/edge/tps"]
	set output [::json::json2dict $apireturn]
	set tps [split [lindex $output 1] " "]
        putserv "PRIVMSG $chan :RR1:[lindex [lindex [lindex $output 1] 1] 1]-\002\00303[lindex [lindex [lindex $output 1] 1] 3] \003\002 || RR2:[lindex [lindex [lindex $output 1] 3] 1]-\002\00303[lindex [lindex [lindex $output 1] 3] 3]\003\002 || Unleashed:[lindex [lindex [lindex $output 1] 5] 1]-\002\00303[lindex [lindex [lindex $output 1] 5] 3] \003\002"
		unset apireturn output tps
	} else {
	   putserv "PRIVMSG $chan :!tps is restricted to Ops or Admins"
       #putserv "NOTICE $nick :RR1 : $rr1tps  || RR2 : $rr2tps || Unleashed : $unletps"
	}
}

proc ote:tpsrr {nick uhost hand chan text} { 
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_" || [ote:adminchk $text] == 1} {
    set apireturn [ote:tpsget "http://dev.otegamers.com/api/v1/edge/tps"]
	set output [::json::json2dict $apireturn]
	set tps [split [lindex $output 1] " "]
        putserv "PRIVMSG $chan :RR1:[lindex [lindex [lindex $output 1] 1] 1]-\002\00303[lindex [lindex [lindex $output 1] 1] 3] \003\002 || RR2:[lindex [lindex [lindex $output 1] 3] 1]-\002\00303[lindex [lindex [lindex $output 1] 3] 3]\003\002"
		unset apireturn output tps
	} else {
	   putserv "PRIVMSG $chan :!tps is restricted to Ops or Admins"
       #putserv "NOTICE $nick :RR1 : $rr1tps  || RR2 : $rr2tps || Unleashed : $unletps"
	}
}
proc ote:tpsftb {nick uhost hand chan text} { 
if {[isop $nick $chan] == 1 ||$nick == "Citidel" || $nick == "Citidel_" || [ote:adminchk $text] == 1} {
    set apireturn [ote:tpsget "http://dev.otegamers.com/api/v1/edge/tps"]
	set output [::json::json2dict $apireturn]
	set tps [split [lindex $output 1] " "]
        putserv "PRIVMSG $chan :Unleashed:[lindex [lindex [lindex $output 1] 5] 1]-\002\00303[lindex [lindex [lindex $output 1] 5] 3] \003\002"
		unset apireturn output tps
	} else {
	   putserv "PRIVMSG $chan :!tps is restricted to Ops or Admins"
       #putserv "NOTICE $nick :RR1 : $rr1tps  || RR2 : $rr2tps || Unleashed : $unletps"
	}
}
#*************************************************
#Mojang Service Status
#*************************************************
proc ote:minechk {nick uhost hand chan text} {     
	set apireturn [ote:tpsget "http://status.mojang.com/check"]
	set output [::json::json2dict $apireturn]
	if {[lindex [lindex $output 2] 1] == "green"} {
		set mjlogin \00303UP\003
	} else {
		set mjlogin \00300,04DOWN\003
	}
	if {[lindex [lindex $output 3] 1] == "green"} {
		set mjsessions \00303UP\003
	} else {
		set mjsessions \00300,04DOWN\003
	}
		if {[lindex [lindex $output 5] 1] == "green"} {
		set mjauth \00303UP\003
	} else {
		set mjauth \00300,04DOWN\003
	}
	putserv "PRIVMSG $chan :Minecaft Service Status: \002[string totitle [lindex [split [lindex [split [lindex $output 2] " "] 0] .] 0] ]: $mjlogin  || [string totitle [lindex [split [lindex [split [lindex $output 3] " "] 0] .] 0] ]: $mjsessions || [string totitle [lindex [split [lindex [split [lindex $output 6] " "] 0] .] 0] ]: $mjauth\002 "
   unset apireturn output
	}



#*************************************************
#UPDATE Command Procedure
#*************************************************
proc ote:update {nick uhost hand chan text} { 
	global upthread
        # Send message to user.
	set apireturn [ote:tpsget "http://dev.otegamers.com/api/v1/edge/version"]
	set output [::json::json2dict $apireturn]
	set tps [split [lindex $output 1] " "]
    putserv "PRIVMSG $chan :RR1 and RR2 Version: [lindex $tps 3]"
    putserv "PRIVMSG $chan :Please go to: $upthread for update info"
	unset apireturn output tps
}
	
proc ote:updateftb {nick uhost hand chan text} { 
	global upthread
        # Send message to user.
	set apireturn [ote:tpsget "http://dev.otegamers.com/api/v1/edge/version"]
	set output [::json::json2dict $apireturn]
	set tps [split [lindex $output 1] " "]
    putserv "PRIVMSG $chan :Unleashed Version: [lindex $tps 5]"
    putserv "PRIVMSG $chan :Please go to: http://www.otegamers.com/topic/6383- for update info"
	unset apireturn output tps
}
proc ote:updatepixel {nick uhost hand chan text} { 
	global upthread
        # Send message to user.
	putserv "PRIVMSG $chan :Please go to: http://www.otegamers.com/topic/7683- for Update and Pack info"
}

#*************************************************
#Help Command Procedure
#*************************************************
proc ote:help {nick uhost hand chan text} { 
        # Send message to user.
        putserv "NOTICE $nick :I am the help bot, this is what I can help you with - Usage: !help mytown"
		putserv "NOTICE $nick :mytown || balance || version || warp || mining || shops"

    }  
#*************************************************
#HELP SUB COMMANDS Command Procedure
#*************************************************
proc ote:mytown {nick uhost hand chan text} {
global wikiMyTown
        # Send message to user.
        putserv "NOTICE $nick :Visit our Wiki on Mytown: $wikiMyTown"
    }	
 proc ote:money {nick uhost hand chan text} { 
        # Send message to user.
        putserv "NOTICE $nick :you can view your in-game cash by typing /balance || you receive $50 every day if you play for 1hour"
    }	

 proc ote:version {nick uhost hand chan text} { 
 global rrver
        # Send message to user.
        putserv "PRIVMSG $chan :RR1 and RR2 are running: $rrver"
    }	
proc ote:warp {nick uhost hand chan text} { 
        # Send message to user.
        putserv "NOTICE $nick :To warp, type /warp spawn || to set a home type /sethome <home name> || to travel there type /home <home name>"
    }
proc ote:mining {nick uhost hand chan text} { 
        # Send message to user.
        putserv "NOTICE $nick :Mining Age info INFORMATION"
    }	
 proc ote:shop {nick uhost hand chan text} { 
        # Send message to user.
        putserv "NOTICE $nick :Shop INFORMATION"
    }
#*************************************************
# Auto message when joining OTEGamers, will send a NOTICE to the person Joining.
#*************************************************
proc join:ote {nick uhost hand chan} {
global rrver
	set apireturn [ote:tpsget "http://dev.otegamers.com/api/v1/edge/version"]
	set output [::json::json2dict $apireturn]
	set tps [split [lindex $output 1] " "]
	putserv "NOTICE $nick :$nick, Welcome to OTEGamers IRC: RR1 and RR2 Version: [lindex $tps 3]  || Unleashed Version: [lindex $tps 5]"
	unset apireturn output tps
}
