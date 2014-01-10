# urltitle.tcl
# Copyright (C) perpleXa 2004-2006
#
# Redistribution, with or without modification, are permitted provided
# that redistributions retain the above copyright notice, this condition
# and the following disclaimer.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY, to the extent permitted by law; without
# even the implied warranty of MERCHANTABILITY or FITNESS FOR A
# PARTICULAR PURPOSE.
 
namespace eval url {
  variable version "3.1";
  variable agent "Opera/8.52 (X11; Linux x86_64; U; en)";
  # Bot will read data in chunks of this size, 8KB is just fine.
  variable readbuf 8192;
  # Read max. 32KB before the connection gets killed.
  # (to prevent the bot from downloading large files when someone pastes shit..)
  variable readlimit 32768;
  variable fds;
  if {![info exists fds]} {
    set fds 0;
  }
  setudef flag urltitle;
  bind pubm -|- * [namespace current]::check;
}
 
proc url::check {nick host hand chan text} {
  
    set text [stripcodes uacgbr $text];
    foreach item [split $text] {
      if {[string match -nocase "http://?*" $item] || [string match -nocase "www.?*" $item] || [string match -nocase "https://?*" $item]} {
        regsub -nocase -- "http(s|)://" [string map [list "\\" "/"] $item] "" url;
        set url [split $url "/"];
        set get [join [lrange $url 1 end] "/"];
        set url [split [lindex $url 0] ":"];
        set host [lindex $url 0]; set port [lindex $url 1];
        if {$port == ""} {set port "80";}
        uconnect $host $port $get $nick $chan;
      }
    }
  
}
 
proc url::uconnect {host port get nick chan} {
  variable agent;
  variable fds;
  variable readbuf;
  set token [namespace current]::[incr fds];
  variable $token;
  upvar 0 $token static;
  array set static {
    data ""
    body 0
    code 0
    sock -1
  }
  if {[catch {set static(sock) [socket -async $host $port]} error]} {
    destroy $token;
    return $error;
  }
  fconfigure $static(sock) -translation {auto crlf} -buffersize $readbuf;
  puts $static(sock) "GET /$get HTTP/1.0";
  puts $static(sock) "Accept: */*";
  if {$port == "80"} {
    puts $static(sock) "Host: $host";
  } else {
    puts $static(sock) "Host: $host:$port";
  }
  puts $static(sock) "User-agent: $agent";
  puts $static(sock) "";
  flush $static(sock);
  fileevent $static(sock) readable [list [namespace current]::handledata $token $nick $chan];
  catch {fconfigure $static(sock) -blocking 0;}
  after [expr 20*1000] [list [namespace current]::destroy $token];
  return $token;
}
 
proc url::handledata {token nick chan} {
  variable readbuf; variable readlimit;
  variable $token;
  upvar 0 $token static;
  if {[eof $static(sock)] || [string length $static(data)]>=$readlimit} {
    destroy $token;
    return;
  }
  set buf [read $static(sock) $readbuf];
  append static(data) $buf;
  foreach line [split $buf "\n"] {
    if {[string match HTTP* $line] && !$static(body)} {
      if {![regexp -- {\d{3}} $line static(code)]} {
        destroy $token;
        return;
      } elseif {$static(code)!=200 && $static(code)!=301 && $static(code)!=302} {
        destroy $token;
        return;
      }
    } elseif {[regexp -nocase -- "^Location:(.+)$" $line -> url]
              && !($static(code)!=301 && $static(code)!=302)} {
      check $nick *!*@* * $chan $url;
      destroy $token;
      return;
    } elseif {[regexp -nocase -- "^Content-type:(.+)$" $line -> type]} {
      if {![string match -nocase text* [string trim $type]]} {
        destroy $token;
        return;
      }
    } elseif {[regexp -nocase -- "^Content-encoding:(.+)$" $line -> encoding]} {
      if {[string match -nocase *gzip* $encoding]
          || [string match -nocase *compress* $encoding]} {
        destroy $token;
        return;
      }
    } elseif {($line == "") && !$static(body)} {
      set static(body) 1;
    } elseif {[regexp -nocase -- {<title>([^<]+?)</title>} $static(data) -> title]
              && $static(code)==200} {
      regsub -all -- {(\n|\r|\s|\t)+} $title " " title;
      set s [expr {[string index $nick end]!="s"?"s":""}];
      puthelp "PRIVMSG $chan :$nick'$s URL title: \"[decode [string trim $title]]\"";
      destroy $token;
      return;
    }
  }
}
 
proc url::destroy {token} {
  variable $token
  upvar 0 $token static;
  if {[info exists static]} {
    catch {fileevent $static(sock) readable "";}
    catch {close $static(sock);}
    unset static;
  }
}
 
proc url::decode {content} {
  if {![string match *&* $content]} {
    return $content;
  }
  set escapes {
    &nbsp; \x20 &quot; \x22 &amp; \x26 &apos; \x27 &ndash; \x2D
    &lt; \x3C &gt; \x3E &tilde; \x7E &euro; \x80 &iexcl; \xA1
    &cent; \xA2 &pound; \xA3 &curren; \xA4 &yen; \xA5 &brvbar; \xA6
    &sect; \xA7 &uml; \xA8 &copy; \xA9 &ordf; \xAA &laquo; \xAB
    &not; \xAC &shy; \xAD &reg; \xAE &hibar; \xAF &deg; \xB0
    &plusmn; \xB1 &sup2; \xB2 &sup3; \xB3 &acute; \xB4 &micro; \xB5
    &para; \xB6 &middot; \xB7 &cedil; \xB8 &sup1; \xB9 &ordm; \xBA
    &raquo; \xBB &frac14; \xBC &frac12; \xBD &frac34; \xBE &iquest; \xBF
    &Agrave; \xC0 &Aacute; \xC1 &Acirc; \xC2 &Atilde; \xC3 &Auml; \xC4
    &Aring; \xC5 &AElig; \xC6 &Ccedil; \xC7 &Egrave; \xC8 &Eacute; \xC9
    &Ecirc; \xCA &Euml; \xCB &Igrave; \xCC &Iacute; \xCD &Icirc; \xCE
    &Iuml; \xCF &ETH; \xD0 &Ntilde; \xD1 &Ograve; \xD2 &Oacute; \xD3
    &Ocirc; \xD4 &Otilde; \xD5 &Ouml; \xD6 &times; \xD7 &Oslash; \xD8
    &Ugrave; \xD9 &Uacute; \xDA &Ucirc; \xDB &Uuml; \xDC &Yacute; \xDD
    &THORN; \xDE &szlig; \xDF &agrave; \xE0 &aacute; \xE1 &acirc; \xE2
    &atilde; \xE3 &auml; \xE4 &aring; \xE5 &aelig; \xE6 &ccedil; \xE7
    &egrave; \xE8 &eacute; \xE9 &ecirc; \xEA &euml; \xEB &igrave; \xEC
    &iacute; \xED &icirc; \xEE &iuml; \xEF &eth; \xF0 &ntilde; \xF1
    &ograve; \xF2 &oacute; \xF3 &ocirc; \xF4 &otilde; \xF5 &ouml; \xF6
    &divide; \xF7 &oslash; \xF8 &ugrave; \xF9 &uacute; \xFA &ucirc; \xFB
    &uuml; \xFC &yacute; \xFD &thorn; \xFE &yuml; \xFF
  };
  set content [string map $escapes $content];
  set content [string map [list "\]" "\\\]" "\[" "\\\[" "\$" "\\\$" "\\" "\\\\"] $content];
  regsub -all -- {&#([[:digit:]]{1,5});} $content {[format %c "\1"]} content;
  regsub -all -- {&#x([[:xdigit:]]{1,4});} $content {[format %c [scan "\1" %x]]} content;
  regsub -all -- {&#?[[:alnum:]]{2,7};} $content "?" content;
  return [subst $content];
}
