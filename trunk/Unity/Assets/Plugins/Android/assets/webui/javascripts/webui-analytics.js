
var OF,$,U;OF.GA={account:'UA-3107048-15',domain:'xp.openfeint.com'};OF.GA.setAccount=function(account,domain){OF.GA.account=account;OF.GA.domain=domain;OF.GA.domainHash=OF.GA.hash(domain);};OF.GA.page=function(url){if(!url){url=OF.page.url;}
OF.GA.sessionViews++;return OF.GA.track(url,'page');};OF.GA.event=function(category,action,label,value){OF.log('GA EVENT: '+category+', '+action+', '+label+', '+value);return OF.GA.track(OF.page.url,'event',category,action,label,value);};OF.GA.trackingUrl=function(url,type,cat,act,lbl,val){var options={};if(url.indexOf('/webui/')!==0){url='/webui/'+url;}
options.utmac=OF.GA.account;options.utmdt=OF.page.title;options.utmcs='UTF-8';options.utmhn=OF.GA.domain;options.utmn=OF.GA.rand();options.utmhid=OF.GA.rand();options.utmp=url.replace(/\?.*$/,'');options.utmr='-';options.utmcc=[];options.utmcc.push('__utma='+[OF.GA.domainHash,OF.GA.cookie.id,OF.GA.cookie.initialDate,OF.GA.cookie.lastDate,OF.GA.now(),OF.GA.sessions].join('.'));options.utmcc.push('__utmb='+[OF.GA.domainHash,OF.GA.sessionViews,10,OF.GA.now()].join('.'));if(OF.GA.sessionViews>1){options.utmcc.push('__utmc='+OF.GA.domainHash);}
options.utmcc.push('__utmz='+[OF.GA.domainHash,OF.GA.cookie.lastDate,OF.GA.sessions,1,['utmccn=(organic)','utmcsr=(organic)','utmctr=-','utmcmd=organic'].join('|')+';'].join('.'));options.utmcc=options.utmcc.join(';+');if(type==='event'){options.utmt=type;cat=cat||'';act=act||'';lbl=lbl||'';val=val||'';options.utme='5('+cat+'*'+act+'*'+lbl+')('+val+')';}
options.utmwv='4.8.6';return'http://www.google-analytics.com/__utm.gif?'+$.urlEncode(options);};OF.GA.track=function(url,type,cat,act,lbl,val){var trackingUrl=OF.GA.trackingUrl(url,type,cat,act,lbl,val);if(OF.GA.enabled){var trackingPixel=new Image();trackingPixel.src=trackingUrl;}
return trackingUrl;};OF.GA.cookie=null;OF.GA.firstEvent=true;OF.GA.init=function(){if(OF.GA.init.complete){return;}
OF.GA.init.complete=true;OF.GA.enabled=OF.isDevice&&OF.settings.enabled&&!OF.disableGA;OF.GA.domainHash=OF.GA.hash(OF.GA.domain);OF.GA.cookie={};OF.GA.sessionStart=OF.GA.now();OF.GA.sessions=0;OF.GA.sessionViews=0;OF.settings.read('ga_cookie_first_session_date',function(val){if(!val){val=OF.GA.now();OF.settings.write('ga_cookie_first_session_date',val);}
OF.GA.cookie.initialDate=val;});OF.settings.read('ga_cookie_last_session_date',function(val){OF.GA.cookie.lastDate=OF.GA.cookie.lastDate||OF.GA.now();OF.settings.write('ga_cookie_last_session_date',OF.GA.sessionStart);});OF.settings.read('ga_cookie_user_id',function(val){if(!val){val=OF.GA.rand();OF.settings.write('ga_cookie_user_id',val.toString());}
OF.GA.cookie.id=val;});OF.settings.read('ga_cookie_sessions',function(val){OF.GA.sessions=(parseInt(val)||0)+1;OF.settings.write('ga_cookie_sessions',OF.GA.sessions);});};OF.GA.init.complete=false;OF.GA.rand=function(){return $.random(1,0x7fffffff);};OF.GA.now=function(){return Math.floor(new Date().getTime()/1000).toString();};OF.GA.hash=function(d){var a=1,c=0,h,o;if(d){a=0;for(h=d.length-1;h>=0;h--){o=d.charCodeAt(h);a=(a<<6&268435455)+o+(o<<14);c=a&266338304;a=c!==0?a^c>>21:a;}}
return a;};OF.pushGACall=OF.GA.push=function(type,b,c,d,e){if(type==='_trackPageview'){OF.GA.page(b);}else if(type==='_trackEvent'){OF.GA.event(b,c,d,e);}};OF.trackEvent=OF.GA.event;