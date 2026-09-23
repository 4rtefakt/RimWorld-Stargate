"""Appels à l'API Mistral (agent avec l'outil image_generation). Clé : variable MISTRAL_API_KEY."""
import json, sys, urllib.request, os
HERE=os.path.dirname(os.path.abspath(__file__))
K=os.environ["MISTRAL_API_KEY"]
AGENT_FILE=os.path.join(HERE,'raw','agent.json')
STYLE=("RimWorld game art style: simple hand-painted 2D sprite, top-down view with a slight 3/4 angle, flat muted colors, soft shading, "
 "thick dark outline, readable silhouette at small size, no text, no letters, no watermark, no shadow on the ground, single object centered, plain pure white background. ")
def req(url, data=None, raw=False):
    r=urllib.request.Request(url, data=json.dumps(data).encode() if data else None, headers={"Authorization":"Bearer "+K,"Content-Type":"application/json"})
    with urllib.request.urlopen(r, timeout=300) as f:
        b=f.read()
    return b if raw else json.loads(b)
def agent_id():
    if os.path.exists(AGENT_FILE): return json.load(open(AGENT_FILE))['id']
    a=req("https://api.mistral.ai/v1/agents",{"model":"mistral-medium-latest","name":"rimworld-stargate-art",
        "instructions":"Generate exactly one image per request with the image generation tool.","tools":[{"type":"image_generation"}]})
    os.makedirs(os.path.dirname(AGENT_FILE),exist_ok=True); json.dump(a,open(AGENT_FILE,'w')); return a['id']
def gen(prompt, out, style=True):
    AG=agent_id()
    d=req("https://api.mistral.ai/v1/conversations",{"agent_id":AG,"inputs":(STYLE if style else "")+prompt,"store":False})
    fid=None
    for o in d.get("outputs",[]):
        for c in (o.get("content") or []) if isinstance(o.get("content"),list) else []:
            if c.get("type")=="tool_file": fid=c["file_id"]
    if not fid:
        print("ÉCHEC", out, json.dumps(d)[:400]); return False
    img=req(f"https://api.mistral.ai/v1/files/{fid}/content", raw=True)
    os.makedirs(os.path.dirname(out), exist_ok=True)
    open(out,"wb").write(img); print("ok", out, len(img)); return True
if __name__=="__main__":
    gen(sys.argv[1], sys.argv[2], style=(len(sys.argv)<4))
