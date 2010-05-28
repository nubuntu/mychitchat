@echo off
IF EXIST JabberMP_UNMERGED.dll del JabberMP_UNMERGED.dll 
ren Jabber.MP.dll JabberMP_UNMERGED.dll 
ilmerge /out:Jabber.MP.dll JabberMP_UNMERGED.dll agsXMPP.dll
