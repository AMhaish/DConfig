/*
** Unobtrusive Ajax support library for jQuery
** Copyright (C) Microsoft Corporation. All rights reserved.
*/
(function(a){var b="unobtrusiveAjaxClick",g="unobtrusiveValidation";function c(d,b){var a=window,c=(d||"").split(".");while(a&&c.length)a=a[c.shift()];if(typeof a==="function")return a;b.push(d);return Function.constructor.apply(null,b)}function d(a){return a==="GET"||a==="POST"}function f(b,a){!d(a)&&b.setRequestHeader("X-HTTP-Method-Override",a)}function h(c,b,e){var d;if(e.indexOf("application/x-javascript")!==-1)return;d=(c.getAttribute("data-ajax-mode")||"").toUpperCase();a(c.getAttribute("data-ajax-update")).each(function(f,c){var e;switch(d){case"BEFORE":e=c.firstChild;a("<div />").html(b).contents().each(function(){c.insertBefore(this,e)});break;case"AFTER":a("<div />").html(b).contents().each(function(){c.appendChild(this)});break;default:a(c).html(b)}})}function e(b,e){var j,k,g,i;j=b.getAttribute("data-ajax-confirm");if(j&&!window.confirm(j))return;k=a(b.getAttribute("data-ajax-loading"));i=b.getAttribute("data-ajax-loading-duration")||0;a.extend(e,{type:b.getAttribute("data-ajax-method")||undefined,url:b.getAttribute("data-ajax-url")||undefined,beforeSend:function(d){var a;f(d,g);a=c(b.getAttribute("data-ajax-begin"),["xhr"]).apply(this,arguments);a!==false&&k.show(i);return a},complete:function(){k.hide(i);c(b.getAttribute("data-ajax-complete"),["xhr","status"]).apply(this,arguments)},success:function(a,e,d){h(b,a,d.getResponseHeader("Content-Type")||"text/html");c(b.getAttribute("data-ajax-success"),["data","status","xhr"]).apply(this,arguments)},error:c(b.getAttribute("data-ajax-failure"),["xhr","status","error"])});e.data.push({name:"X-Requested-With",value:"XMLHttpRequest"});g=e.type.toUpperCase();if(!d(g)){e.type="POST";e.data.push({name:"X-HTTP-Method-Override",value:g})}a.ajax(e)}function i(c){var b=a(c).data(g);return!b||!b.validate||b.validate()}a("a[data-ajax=true]").live("click",function(a){a.preventDefault();e(this,{url:this.href,type:"GET",data:[]})});a("form[data-ajax=true] input[type=image]").live("click",function(c){var g=c.target.name,d=a(c.target),f=d.parents("form")[0],e=d.offset();a(f).data(b,[{name:g+".x",value:Math.round(c.pageX-e.left)},{name:g+".y",value:Math.round(c.pageY-e.top)}]);setTimeout(function(){a(f).removeData(b)},0)});a("form[data-ajax=true] :submit").live("click",function(c){var e=c.target.name,d=a(c.target).parents("form")[0];a(d).data(b,e?[{name:e,value:c.target.value}]:[]);setTimeout(function(){a(d).removeData(b)},0)});a("form[data-ajax=true]").live("submit",function(d){var c=a(this).data(b)||[];d.preventDefault();if(!i(this))return;e(this,{url:this.action,type:this.method||"GET",data:c.concat(a(this).serializeArray())})})})(jQuery);
