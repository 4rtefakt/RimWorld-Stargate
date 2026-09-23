import json, os
from collections import deque
from PIL import Image, ImageFilter
HERE=os.path.dirname(os.path.abspath(__file__))
ROOT=os.path.join(HERE,'..','..','..')+'/'
spec=json.load(open(os.path.join(HERE,'raw','spec.json')))
ROT={'sctop':-90}
CENTER=False
def remove_white_bg(im):
    im=im.convert('RGBA'); w,h=im.size; px=im.load()
    def bg(c): r,g,b,_=c; return min(r,g,b)>218 and max(r,g,b)-min(r,g,b)<30
    seen=bytearray(w*h); q=deque()
    for x in range(w):
        for y in (0,h-1): q.append((x,y))
    if CENTER: q.append((w//2,h//2))  # fond enclos au centre (intérieur de la Porte)
    for y in range(h):
        for x in (0,w-1): q.append((x,y))
    while q:
        x,y=q.popleft(); i=y*w+x
        if seen[i] or not bg(px[x,y]): continue
        seen[i]=1
        for dx,dy in ((1,0),(-1,0),(0,1),(0,-1)):
            nx,ny=x+dx,y+dy
            if 0<=nx<w and 0<=ny<h and not seen[ny*w+nx]: q.append((nx,ny))
    a=Image.new('L',(w,h),255); ap=a.load()
    for y in range(h):
        for x in range(w):
            if seen[y*w+x]: ap[x,y]=0
    a=a.filter(ImageFilter.MinFilter(3)).filter(ImageFilter.GaussianBlur(1.2))
    im.putalpha(a); return im
def icon_alpha(im):
    g=im.convert('L'); w,h=g.size
    # retire des bandes claires en bordure si le centre est sombre (ex. barres blanches)
    center=g.crop((w//4,h//4,3*w//4,3*h//4))
    border=[g.getpixel((x,0)) for x in range(w)]+[g.getpixel((0,y)) for y in range(h)]
    if sum(border)/len(border)>128 and sum(center.getdata())/(center.size[0]*center.size[1])<128:
        rows=[y for y in range(h) if sum(g.getpixel((x,y)) for x in range(0,w,8))/(w/8)<128]
        g=g.crop((0,rows[0],w,rows[-1]+1)); w,h=g.size
    border=[g.getpixel((x,0)) for x in range(w)]+[g.getpixel((0,y)) for y in range(h)]
    if sum(border)/len(border)>128: g=Image.eval(g,lambda v:255-v)
    a=Image.eval(g,lambda v:0 if v<40 else min(255,int((v-40)*255/170)))
    out=Image.new('RGBA',g.size,(255,255,255,0)); out.putalpha(a); return out
def finish(im,size,margin=0.06):
    bb=im.getchannel('A').point(lambda v:255 if v>12 else 0).getbbox()
    im=im.crop(bb); w,h=im.size; s=int(max(w,h)*(1+2*margin))
    c=Image.new('RGBA',(s,s),(0,0,0,0)); c.paste(im,((s-w)//2,(s-h)//2)); return c.resize((size,size),Image.LANCZOS)
for i,p,mode,out,size in spec:
    im=Image.open(os.path.join(HERE,'raw',f'{i}.png'))
    if i=='x_Asgard':
        w,h=im.size; im=im.crop((int(w*.2),int(h*.2),int(w*.8),int(h*.8)))
    CENTER = i in ('gate',)
    im=remove_white_bg(im) if mode=='obj' else icon_alpha(im)
    if i in ROT: im=im.rotate(ROT[i],expand=True)
    res=finish(im,size)
    path=ROOT+out+'.png'; os.makedirs(os.path.dirname(path),exist_ok=True); res.save(path)
print('traité',len(spec))
