if (typeof(Sitecore)== 'undefined')
    Sitecore = new Object();
Sitecore.PlaceholderManager = new function() {

    Sitecore.Dhtml.attachEvent(window, "onload", function() { Sitecore.PlaceholderManager.load() } );
    this.__PLACEHOLDER = null;
    this.__LIST = null;
       
    /* onload */
    this.load = function(sender, evt) {
        this.__PLACEHOLDER = $('__PLACEHOLDER');        
        this.__LIST = $('__LIST');               
        
        if (window.top.Sitecore == null) {
            window.top.Sitecore = Sitecore;
        }
        
        window.top.Sitecore.PlaceholderManager = Sitecore.PlaceholderManager;
        
        var doc =  this.__PLACEHOLDER.contentDocument == null ? 
                   this.__PLACEHOLDER.contentWindow.document :
                   this.__PLACEHOLDER.contentDocument; 

        if (doc.getElementById('scPalettePlaceholdersList') != null) {                         
            this.__LIST.innerHTML = doc.getElementById('scPalettePlaceholdersList').innerHTML;                                   
            this.updateEvent();
            
            var placeholders = $$('#__LIST a');
            if (placeholders.length == 0){
               this.__LIST.innerHTML = "<div class='scEmptyContentListText'>(1)There are no allowed placeholders in order to insert a new form.</div>";
               $('__LISTACTIVE').value = "";
            }else{
               this.onPlaceholderClick(null, event, "", "");            
            }                       
        }else{
            this.__LIST.innerHTML = "<div class='scEmptyContentListText'>(2) There are no allowed placeholders in order to insert a new form.</div>";
        }
    }
    
    
    this.updateEvent = function(){
            
        var placeholders = $$('#__LIST a');
        
        placeholders.each(
            function(element){

                var names = [];
                clear(element.id, '_', ' ').scan(/\w+/, 
                        function(match){
                            names.push(match[0]);                            
                        })
                        
                var _uniqid = "";
                var uniqid = "";                
                if (names.length > 1){
                    for(var i = 1; i < names.length; ++i) {
                        _uniqid += ("_" + names[i]);
                        uniqid += ("/" + names[i]);
                    };
                }
                
                              
               element.onmouseout = null;
               element.onmouseover = null;
               element.onclick = null;
               element.style.width = "100%";
               
               var activeName = $$("#" + element.id + " div[class=scPalettePlaceholderTitle]")[0].innerHTML;
               
               Sitecore.Dhtml.attachEvent(element, "onmouseout", function() { Sitecore.PlaceholderManager.highlightPlaceholder(element, event, _uniqid) } );
               Sitecore.Dhtml.attachEvent(element, "onmouseover", function() { Sitecore.PlaceholderManager.highlightPlaceholder(element, event, _uniqid) } );
               Sitecore.Dhtml.attachEvent(element, "onclick", function() { Sitecore.PlaceholderManager.onPlaceholderClick(element, event, uniqid, activeName) } );                       
            
            });
    }
    
    this.highlightPlaceholder = function(element, evt, id) {
        var placeholder = $(id);
  
        if (placeholder != null) {
            if (evt.type == "mouseover") {
                placeholder.show();
            }
            else {
                placeholder.hide();
            }
        }
  
        this.showTooltip(element, evt);
    }
    
    this.onPlaceholderClick = function(element, evt, placeholder, name) {
        
        if (placeholder == null || placeholder == "") {
            if ($('__LISTVALUE').value == "") {
                if (this.__LIST.firstChild != null) {
                    var names = [];
                    clear(this.__LIST.firstChild.id, '_', ' ').scan(/\w+/, 
                            function(match){
                                names.push(match[0]);                            
                            })
                            
                    var uniqid = "";                
                    if (names.length > 1){
                        for(var i = 1; i < names.length; ++i) {
                            uniqid += ("/" + names[i]);
                        };
                    } 
                    $('__LISTVALUE').value = uniqid;
                    placeholder = uniqid;
                    
                    var activeName = $$("#" + this.__LIST.firstChild.id + " div[class=scPalettePlaceholderTitle]")[0];
                    $('__LISTACTIVE').value = activeName.innerHTML;
                }
            }
            else
            {
                 placeholder = $('__LISTVALUE').value;
            }
          
        } else {
            $('__LISTVALUE').value = placeholder;                                  
            $('__LISTACTIVE').value = name;                     
        }
     
        if (placeholder != null && placeholder != "") {     
        
            var names = [];
            clear(placeholder, '/', ' ').scan(/\w+/, 
                    function(match){
                        names.push(match[0]);                            
                    })
            var shortName = "ph";
            var longName = "ph_";
            
            if (names.length > 0){
                for(var i = 0; i < names.length; ++i) {
                    longName += ("_" + names[i]);
                    shortName += ("_" + names[i]);
                };
            }        
        
            var ctrl = $(longName) == null ? $(shortName) : $(longName);
            
            $A(ctrl.up().childNodes).each(function(e) { 
                e.className = "scPalettePlaceholder";
            });
        
            ctrl.className = "scPalettePlaceholderSelected";  
        }
    }
    
    this.showTooltip = function(element, evt) {
        var tooltip = $(element.lastChild);
        var x = Event.pointerX(evt);
        var y = Event.pointerY(evt);

        if (evt.type == "mouseover") {
            if (tooltip.style.display == "none") {
              clearTimeout(this.tooltipTimer);
              
              this.tooltipTimer = setTimeout(function() {
                var html = tooltip.innerHTML;
                
                if (html == "") {
                  return;
                }
              
                var t = $("scCurrentTooltip");
                if (t == null) {
                  t = new Element("div", { "id":"scCurrentTooltip", "class": "scPalettePlaceholderTooltip", "style": "display:none" });
                  document.body.appendChild(t);
                }
                
                t.innerHTML = html;
              
                t.style.left = x + "px";
                t.style.top = y + "px";
                t.style.display = "";
              }, 450);
            }
        }
        else {
            clearTimeout(this.tooltipTimer);
            var t = $("scCurrentTooltip");
            if (t != null) {
              t.style.display = "none";
            }
        }
    } 
    function clear(value, oldChar, newChar){
    var _clear = "";
    for (var i = 0; i < value.length; ++i){
        if (value.charAt(i) == oldChar){
            _clear += newChar;
        }else{
            _clear += value.charAt(i);
        }        
    }
    return _clear;
    }   

}
