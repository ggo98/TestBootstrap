const baseUrl = "https://localhost/dvweb/ddi";

const map = new Map();

async function resolve() {
}

async function ddiapi(endpoint)
{
    try {
        //endpoint = baseUrl + encodeURIComponent(endpoint);
        endpoint = "ApiProxy.ashx?q=" + encodeURIComponent(endpoint);

        document.body.style.cursor = 'wait'; // does not work
        //document.getElementById("mainDiv").style.cursor = "wait";
        //await new Promise(resolve => setTimeout(resolve, 1500));
        //alert("pause");
        var response = await fetch(endpoint,
            {
                method: 'GET',
                headers:
                {
                    'Accept': 'application/json'
                },
                credentials: 'include'
            });

        if (!response.ok) {
            throw new Error("HTTP " + response.status + ": " + response.statusText);
        }

        return await response.json();
    }
    catch (e) {
        console.error(e);
        throw new Error(e.message);
    }
    finally {
        document.body.style.cursor = 'default';
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
        endpoint = "/tables/" + xendpoint;
        try
        {
            var lst = document.getElementById('lst');
            console.log("endpoint = "+ endpoint)
            var data = await ddiapi(endpoint);

            var info = await ddiapi("/info/" + "Secured SQL Server");
            alert(info);
            map.set(alias, info);

            lst.replaceChildren();
            for (const item of data) {
                //data.forEach(item => {
                var li = document.createElement('li');
                var path = item["Table Path"];
                console.log("TABLE PATH: " + path);
                //var value = path.replaceAll(".", "/");
                var tmp = xendpoint.replaceAll("/", ".");
                const matches = tmp.match(/"[^"]*"|[^".]+/g) || [];
                var alias = matches[0];
                await this.getAndStoreInfoInMap(alias);

                value = alias + "/" + path.replaceAll(".", "/");
                console.log("value: " + value);

                li.dataset.item = value;
                console.log("tables, adding " + li.dataset.item);
                path = path.replaceAll('"', '');
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
        var text = document.getElementById('path').value;
        text = text.replaceAll(".", "/");
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
            path = path.replaceAll(".", "/");
            console.log('calling tables, path:', path);
            await this.tables(value);
            console.log("-----------------------------------------------------------");
        });

        await this.aliases();
    }

};

document.addEventListener('DOMContentLoaded', () => app.init());
