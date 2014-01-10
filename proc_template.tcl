#basic Bind, searches IRC text in the channel for the command
bind pubm - "IRCCOMMAND" PROCCOMMAND

#basic Proc, for the pubm bind.
proc PROCCOMMAND {nick uhost hand chan text} {
 #spacing is super important... 
 #code goes here
 putserv "PRIVMSG <$chan | $nick> : Hello World!"
 #putserv - Sends the message to the IRC queue, putquick and puthelp are also available and are different queues
 #PRIVMSG - sends the message as a normal IRC message can be sent to $nick or $chan
 # <$chan | $nick> - either, $chan sends to whole channel $nick sends as a reply\pm
 # " : " seperates the command from the text. the trailing space can be deleted.

} #end your proc

