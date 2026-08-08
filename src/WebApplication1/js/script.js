import Color from './color.js';

const baseUrl = "http://localhost/dvweb_dbg/ddi";

const map = new Map();

async function resolve() {
}

function cleanPath(s) {
    console.log("CLEANPATH(" + s+  ")");
    var ret = "/"
        + s.split('/') // "/tables//a b c/" => ['', 'tables', '', 'a b c', ''] 
            .filter(Boolean) // => ['tables', 'a b c']
            .join('/'); // => "tables/a b c"
    if ("/" == ret)
        ret = "";
    console.log("RET=" + ret);
    return ret;
}

function myReplaceAll(s, pattern, replacement) {
    if (null == s)
        return "";
    return s.replaceAll(pattern, replacement);
}

async function ddiapi(endpoint) {
    try {
        var errorTxt = document.getElementById("errorTxt");
        errorTxt.textContent = "";
        endpoint = cleanPath(endpoint.trim());

        // "pure javascript"" version
        endpoint = baseUrl + endpoint;

        // "with ApiProxy"" version
        //endpoint = "ApiProxy.ashx?q=" + encodeURIComponent(endpoint);

        console.log("");
        console.log("*** DDIAPI: " + endpoint);
        console.log("");

        //document.body.style.cursor = 'wait'; // does not work
        //document.getElementById("mainDiv").style.cursor = "wait";
        //await new Promise(resolve => setTimeout(resolve, 1500));
        var response = await fetch(endpoint,
        {
            method: 'GET',
            headers:
            {
                'Accept': 'application/json'
            },
            credentials: 'include' // required for the "pure javascript"" version
        });

        if (!response.ok) {
            var json = await response.text();
            //var json = JSON.stringify(await response.json(), null, 4);
            if (null != json) {
                //const position = [...json].findIndex(char => char === "\n");
                //alert(position); // 4
                document.getElementById("errorTxt").textContent = json.replaceAll("\r\n", "\n").replaceAll("<br/>");
                throw new Error(json);
            }
            throw new Error("HTTP " + response.status + ": " + response.statusText);
        }

        var ret = await response.json();
        return ret;
    }
    catch (e) {
        console.error(e.message);
        throw new Error(e.message);
    }
    finally {
        //document.body.style.cursor = 'default';
        //document.getElementById("mainDiv").style.cursor = "default";
    }
}

async function handleClick() {
    console.log('call navigateClick');
    await app.navigateClick();
}
window.handleClick = handleClick; // required because script.js is included as a module in index.html (<script type="module" src="js/script.js"></script>)

async function delayedRequestAnimationFrame(delay) {
    requestAnimationFrame(resolve);
    await new Promise(resolve => setTimeout(resolve, delay));
}

const app = 
{
    async getAndStoreInfoInMap(alias) {
        if (!map.has(alias)) {
            var info = await ddiapi("/info/" + alias);
            //console.log(JSON.stringify(info));
            map.set(alias, info);
        }
    },

    // /Secured SQL Server/DataSetReport/demo
    async tables(xendpoint)
    { 
        console.log("xendpoint: " + xendpoint);
        if (null != xendpoint)
            xendpoint = xendpoint.trim();

        if (null == xendpoint
            || "" == xendpoint)
            return await this.aliases();

        var endpoint = "/tables/" + xendpoint;
        try
        {
            var lst = document.getElementById('lst');
            console.log("endpoint = "+ endpoint)
            var data = await ddiapi(endpoint);

            lst.replaceChildren();
            for (const item of data) {
                //data.forEach(item => {
                var li = document.createElement('li');
                var path = item["Table Path"];
                console.log("TABLE PATH: " + path);
                var tmp = myReplaceAll(xendpoint, "/", ".");
                const matches = tmp.match(/"[^"]*"|[^".]+/g) || [];
                var alias = matches[0];
                await this.getAndStoreInfoInMap(alias);

                var value = alias + "/" + myReplaceAll(path, ".", "/");
                console.log("value: " + value);

                li.dataset.item = value;
                console.log("tables, adding " + li.dataset.item);
                path = myReplaceAll(path, '"', '');
                li.textContent = path;
                lst.appendChild(li);
            }
            //});
        }
        catch (e)
        {
            //alert(e);
            console.error(e);
        }
        finally
        {
        }
        console.log("-----------------------------------------------------------");
    },

    async aliases() {
        try {
            var lst = document.getElementById('lst');
            var data = await ddiapi("/aliases");

            lst.replaceChildren();
            data.forEach(item => {
                var li = document.createElement('li');
                li.dataset.item = JSON.stringify(item);
                console.log("aliases, adding " + li.dataset.item);
                li.textContent = item;
                lst.appendChild(li);
            });
        }
        catch (e) {
            alert(e);
            console.error(e);
        }
        finally {
        }
        console.log("-----------------------------------------------------------");
    },

    async navigateClick() {
        console.log("navigateClick");
        var text = document.getElementById('path').value.trim();
        text = cleanPath(text);
        if ("/" == text)
            text = "";
        text = myReplaceAll(text, ".", "/");
        console.log('text:', text);
        await this.tables(text);
        console.log("-----------------------------------------------------------");
    },

    getEffectiveBackgroundColor(el) {

        //const colorScheme = getComputedStyle(document.documentElement).colorScheme;
        //const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        //if (colorScheme.includes('dark') || (colorScheme.includes('light dark') && prefersDark))
        //{
        //    alert('DARK mode');
        //    return 'rgb(18, 18, 18)'; // approximate dark canvas default (varies)
        //}
        //else
        //    alert('NOT DARK mode');

        let current = el;
        while (current) {
            const bg = getComputedStyle(current).backgroundColor;

            // Check if it's not transparent
            const isTransparent =
                bg === "transparent" ||
                bg === "rgba(0, 0, 0, 0)" ||
                /rgba\(.*,\s*0\s*\)/.test(bg); // catches any alpha of 0

            if (!isTransparent) {
                return bg;
            }
            current = current.parentElement;
        }
        return "rgb(255, 255, 255)";
    },

    swapColorAndBackground(item, oldColors) {
        //return null;
        if (null != oldColors) {
            item.style.color = oldColors[0];
            item.style.backgroundColor = oldColors[1];
            return oldColors;
        }

        var currentColor = getComputedStyle(item).color;
        var currentBackground = getComputedStyle(item).backgroundColor;
        var ret = [currentColor, Color.fromString(currentBackground)];

        currentBackground = this.getEffectiveBackgroundColor(item);

        var tmp = Color.fromString(currentBackground);
        currentBackground = tmp.toString(true);
        item.style.color = currentBackground;
        item.style.backgroundColor = currentColor;
        return ret;
    },

    async init() {
        var lst = document.getElementById('lst');
        lst.addEventListener('click', async (event) => {
            var item = event.target.closest('li');
            if (!item)
                return;
            var value = item.dataset.item;
            var isAlias = item.isAlias;
            console.log("in listener1: value=" + value);
            var path = item.textContent.trim();
            path = myReplaceAll(path, ".", "/");
            console.log('calling tables, path:', path);
            var oldColors;
            try {
                oldColors = this.swapColorAndBackground(item);
                //item.style.color = "rgb(255,0,0)"

                //item.style.color = "rgb(255, 255, 255)";//currentBackground;
                //item.style.backgroundColor = "rgb(0, 0, 0)";//currentBackground;

                await new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));
                await new Promise(resolve => setTimeout(resolve, 250));

                await this.tables(value);
            }
            finally {
                oldColors = this.swapColorAndBackground(item, oldColors);
            }
            console.log("-----------------------------------------------------------");
        });

        await this.aliases();
    }

};

document.addEventListener('DOMContentLoaded', () => app.init());
