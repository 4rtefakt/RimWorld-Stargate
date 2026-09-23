import json, os, sys
from gen import gen
from batch import ICON
T='Textures/'
GLOW=("Game VFX sprite on a pure black background, no text, no border, single effect centered, horizontal, pointing right. Subject: ")
B=[
 ('c_enter','obj',"a game button icon: a glowing blue shimmering water-like event horizon filling a stone ring with chevrons, a small arrow pointing into it",T+'UI/Commands/SG_StargateEnter',128),
 ('c_dial','obj',"a game button icon: a round stone dialing console seen from above with glyph buttons and a glowing red central dome, a hand finger pressing one glyph",T+'UI/Commands/SG_StargateDial',128),
 ('c_rings','obj',"a game button icon: five stacked golden transport rings falling from above onto a round golden platform, a column of white light between them",T+'UI/Commands/SG_ActivateRings',128),
 ('m_cult','icon',ICON+"a cobra rising behind a pyramid with glowing eyes",T+'UI/Memes/SG_GoauldWorship',128),
 ('x_TokraQueen','icon',ICON+"a calm female head in profile wearing a small crown, a serpent behind the neck",T+'UI/Icons/Xenotypes/SG_TokraQueen',64),
 ('sarc','obj',"orthographic view from directly above of an ornate golden Egyptian sarcophagus bed, long rectangular box oriented vertically in the image, lid closed, engraved hieroglyphs, blue and gold stripes, a stylized pharaoh head at the top end, seen straight down, no perspective, no shadow",T+'Things/Building/SG_Sarcophagus/SG_Sarcophagus',256),
 ('p_staff','glow',GLOW+"a fiery orange-yellow plasma bolt, bright white-hot core with an orange glow and a short tapered tail trailing left",T+'Things/Projectile/SG_StaffPlasma',64),
 ('p_zat','glow',GLOW+"a crackling blue electric energy ball with small lightning arcs, bright white core",T+'Things/Projectile/SG_ZatBolt',64),
 ('p_kara','glow',GLOW+"a curved crescent-shaped golden-orange shockwave ripple, bright core fading at the edges",T+'Things/Projectile/SG_KaraKeshWave',64),
]
if __name__=="__main__":
    only=set(sys.argv[1:])
    for i,mode,p,out,size in B:
        if only and i not in only: continue
        raw=os.path.join(os.path.dirname(os.path.abspath(__file__)),'raw',f'{i}.png')
        if os.path.exists(raw): continue
        for _ in range(3):
            try:
                if gen(p,raw,style=(mode=='obj')): break
            except Exception as e:
                print('err',i,e); import time; time.sleep(20)
    json.dump(B,open(os.path.join(os.path.dirname(os.path.abspath(__file__)),'raw','spec2.json'),'w'))
