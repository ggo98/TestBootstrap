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
        //alert("pause");
        //alert(endpoint);
        var response = await fetch(endpoint,
        {
            method: 'GET',
            headers:
            {
                'Accept': 'application/json'
            },
            credentials: 'include' // required for the pure javascript version
        });

        if (!response.ok) {
            throw new Error("HTTP " + response.status + ": " + response.statusText);
        }

        var ret = await response.json();
        //alert(ret);
        return ret;
    }
    catch (e) {
        console.error(e);
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

const app = 
{
    async getAndStoreInfoInMap(alias) {
        if (!map.has(alias)) {
            var info = await ddiapi("/info/" + alias);
            //alert(info.MinLevel);
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

        endpoint = "/tables/" + xendpoint;
        try
        {
            var lst = document.getElementById('lst');
            console.log("endpoint = "+ endpoint)
            var data = await ddiapi(endpoint);

            //var info = await ddiapi("/info/" + "Secured SQL Server");
            //alert(info);
            //map.set(alias, info);

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

                value = alias + "/" + myReplaceAll(path, ".", "/");
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
            alert(e);
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
            await this.tables(value);
            console.log("-----------------------------------------------------------");
        });

        await this.aliases();
    }

};

document.addEventListener('DOMContentLoaded', () => app.init());
