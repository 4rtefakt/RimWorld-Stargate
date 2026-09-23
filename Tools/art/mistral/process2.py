import json, os
from PIL import Image
import process as P  # réutilise remove_white_bg, icon_alpha, finish (le module retraite aussi spec 1, sans effet)
ROOT=P.ROOT
spec=json.load(open(os.path.join(P.HERE,'raw','spec2.json')))
def glow(im):
    im=im.convert('RGB'); w,h=im.size; px=im.load()
    out=Image.new('RGBA',(w,h)); op=out.load()
    for y in range(h):
        for x in range(w):
            r,g,b=px[x,y]; a=max(r,g,b)
            a2=0 if a<18 else a
            op[x,y]=(min(255,r*255//a) if a else 0,min(255,g*255//a) if a else 0,min(255,b*255//a) if a else 0,a2)
    return out
def save(im,out):
    import os; p=ROOT+out+'.png'; os.makedirs(os.path.dirname(p),exist_ok=True); im.save(p); print('->',out)
for i,mode,p,out,size in spec:
    im=Image.open(os.path.join(P.HERE,'raw',f'{i}.png'))
    if mode=='glow':
        im=glow(im).rotate(90,expand=True)  # les projectiles RimWorld pointent vers le haut
        save(P.finish(im,size,0.02),out)
    elif mode=='icon':
        save(P.finish(P.icon_alpha(im),size),out)
    elif i=='sarc':
        P.CENTER=False; im=P.remove_white_bg(im)
        im=im.crop(im.getchannel('A').point(lambda v:255 if v>12 else 0).getbbox())
        H=int(size*0.96); W=min(int(size*0.5),int(im.width*H/im.height))
        im=im.resize((W,H),Image.LANCZOS)
        c=Image.new('RGBA',(size,size),(0,0,0,0)); c.paste(im,((size-W)//2,(size-H)//2))
        save(c,out+'_north'); save(c.rotate(-90),out+'_east'); save(c.rotate(180),out+'_south')
    else:
        P.CENTER=False; save(P.finish(P.remove_white_bg(im),size),out)
