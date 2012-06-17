using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xnamame036.mame
{
    partial class Mame
    {

        /* dirty flag */
        public const byte GFXOBJ_DIRTY_ALL = 0xff;
        public const byte GFXOBJ_DIRTY_SX_SY = 0xff;
        public const byte GFXOBJ_DIRTY_SIZE = 0xff;
        public const byte GFXOBJ_DIRTY_PRIORITY = 0xff;
        public const byte GFXOBJ_DIRTY_CODE = 0xff;
        public const byte GFXOBJ_DIRTY_COLOR = 0xff;
        public const byte GFXOBJ_DIRTY_FLIP = 0xff;

        /* sort(priority) flag */
        public const byte GFXOBJ_DONT_SORT = 0x00;
        public const byte GFXOBJ_DO_SORT = 0x01;
        public const byte GFXOBJ_SORT_OBJECT_BACK = 0x02;
        public const byte GFXOBJ_SORT_PRIORITY_BACK = 0x04;

        public const byte GFXOBJ_SORT_DEFAULT = GFXOBJ_DO_SORT;
        public delegate void _special_handler(osd_bitmap bitmap, gfx_object _object);
        public class gfx_object
        {
            public int transparency;		/* transparency of gfx */
            public int transparent_color;	/* transparet color of gfx */
            public GfxElement gfx;	/* source gfx , if gfx==0 then not calcrate sx,sy,visible,clip */
            public int code;				/* code of gfx */
            public int color;				/* color of gfx */
            public int priority;			/* priority 0=lower */
            public int sx;					/* x position */
            public int sy;					/* y position */
            public int flipx;				/* x flip */
            public int flipy;				/* y flip */
            /* source window in gfx tile : only non zooming gfx */
            /* if use zooming gfx , top,left should be set 0, */
            /* and width,height should be set same as gfx element */
            public int top;					/* x offset of source data */
            public int left;					/* y offset of source data */
            public int width;				/* x size */
            public int height;				/* y size */
            public int palette_flag;		/* !! not supported !! , palette usage flag tracking */
            /* zooming */
            public int scalex;					/* zommscale , if 0 then non zooming gfx */
            public int scaley;					/* */
            /* link */
            public gfx_object next, prev;	/* next object point */
            /* exrernal draw handler , (for tilemap,special sprite,etc) */
            public _special_handler special_handler;
            /* !!! not suppored yet !!! */
            public int dirty_flag;			/* dirty flag */
            /* !! only drawing routine !! */
            public bool visible;		/* visible flag        */
            public int draw_x;			/* x adjusted position */
            public int draw_y;			/* y adjusted position */
            public rectangle clip; /* clipping object size with visible area */
        }

        /* object list */
        public class gfx_object_list
        {
            public int nums;						/* read only */
            public int max_priority;				/* read only */
            public gfx_object[] objects;
            /* priority : objects[0]=lower       */
            /*          : objects[nums-1]=higher */
            public gfx_object first_object; /* pointer of first(lower) link object */
            /* !! private area !! */
            public int sort_type;					/* priority order type */
            public gfx_object_list next;	/* resource tracking */
        }
        static gfx_object_list first_object_list;
        const byte MAX_PRIORITY = 16;

        void gfxobj_init()
        {
            first_object_list = null;
        }
        void gfxobj_close()
        {
            gfx_object_list object_list, object_next;
            for (object_list = first_object_list; object_list != null; object_list = object_next)
            {
                object_list.objects = null;
                object_next = object_list.next;
                object_list = null;
            }
        }
        public static gfx_object_list gfxobj_create(int nums, int max_priority, gfx_object def_object)
        {
            gfx_object_list object_list;

            /* priority limit check */
            if (max_priority >= MAX_PRIORITY)
                return null;

            /* allocate object liset */
            object_list = new gfx_object_list();

            //memset(object_list,0,sizeof(struct gfx_object_list));

            /* allocate objects */
            object_list.objects = new gfx_object[nums];

            if (def_object == null)
            {	/* clear all objects */
                for (int i = 0; i < nums; i++)
                    object_list.objects[i] = new gfx_object();//memset(object_list.objects,0,nums*sizeof(struct gfx_object));
            }
            else
            {	/* preset with default objects */
                for (int i = 0; i < nums; i++)
                    object_list.objects[i] = def_object;
            }
            /* setup objects */
            for (int i = 0; i < nums; i++)
            {
                /*	dirty flag */
                object_list.objects[i].dirty_flag = GFXOBJ_DIRTY_ALL;
                /* link map */
                object_list.objects[i].next = i < nums - 1 ? object_list.objects[i + 1] : null;
                object_list.objects[i].prev = i > 0 ? object_list.objects[i - 1] : null;
            }
            /* setup object_list */
            object_list.max_priority = max_priority;
            object_list.nums = nums;
            object_list.first_object = object_list.objects[0]; /* top of link */
            object_list.objects[nums - 1].next = null; /* bottom of link */
            object_list.sort_type = GFXOBJ_SORT_DEFAULT;
            /* resource tracking */
            object_list.next = first_object_list;
            first_object_list = object_list;

            return object_list;
        }
        static void object_update(gfx_object _object)
        {
            int min_x, min_y, max_x, max_y;

            /* clear dirty flag */
            _object.dirty_flag = 0;

            /* if gfx == 0 ,then bypass (for special_handler ) */
            if (_object.gfx == null)
                return;

            /* check visible area */
            min_x = Machine.drv.visible_area.min_x;
            max_x = Machine.drv.visible_area.max_x;
            min_y = Machine.drv.visible_area.min_y;
            max_y = Machine.drv.visible_area.max_y;
            if (
                (_object.width == 0) ||
                (_object.height == 0) ||
                (_object.sx > max_x) ||
                (_object.sy > max_y) ||
                (_object.sx + _object.width <= min_x) ||
                (_object.sy + _object.height <= min_y))
            {	/* outside of visible area */
                _object.visible = false;
                return;
            }
            _object.visible = true;
            /* set draw position with adjust source offset */
            _object.draw_x = _object.sx -
                (_object.flipx != 0 ?
                    (_object.gfx.width - (_object.left + _object.width)) : /* flip */
                    (_object.left) /* non flip */
                );

            _object.draw_y = _object.sy -
                (_object.flipy != 0 ?
                    (_object.gfx.height - (_object.top + _object.height)) : /* flip */
                    (_object.top) /* non flip */
                );
            /* set clipping point to object draw area */
            _object.clip.min_x = _object.sx;
            _object.clip.max_x = _object.sx + _object.width - 1;
            _object.clip.min_y = _object.sy;
            _object.clip.max_y = _object.sy + _object.height - 1;
            /* adjust clipping point with visible area */
            if (_object.clip.min_x < min_x) _object.clip.min_x = min_x;
            if (_object.clip.max_x > max_x) _object.clip.max_x = max_x;
            if (_object.clip.min_y < min_y) _object.clip.min_y = min_y;
            if (_object.clip.max_y > max_y) _object.clip.max_y = max_y;
        }

        static void gfxobj_update_one(gfx_object_list object_list)
        {
            throw new Exception();
#if true
            gfx_object _object;
            gfx_object start_object, last_object;
            int dx, start_priority, end_priority;
            int priorities = object_list.max_priority;
            int priority;

            if ((object_list.sort_type & GFXOBJ_DO_SORT) != 0)
            {
                gfx_object[] top_object = new gfx_object[MAX_PRIORITY], end_object = new gfx_object[MAX_PRIORITY];
                /* object sort direction */
                if ((object_list.sort_type & GFXOBJ_SORT_OBJECT_BACK) != 0)
                {
                    start_object = object_list.objects[object_list.nums - 1];
                    last_object = object_list.objects[0].prev;
                    dx = -1;
                }
                else
                {
                    start_object = object_list.objects[0];
                    last_object = object_list.objects[object_list.nums];
                    dx = 1;
                }
                /* reset each priority point */
                for (priority = 0; priority < priorities; priority++)
                    end_object[priority] = null;
                /* update and sort */
                for (_object = start_object; _object != last_object; )
                {
                    /* update all objects */
                    if (_object.dirty_flag != 0)
                        object_update(_object);
                    /* store link */
                    if (_object.visible)
                    {
                        priority = _object.priority;
                        if (end_object[priority] != null)
                            end_object[priority].next = _object;
                        else
                            top_object[priority] = _object;
                        end_object[priority] = _object;
                    }
                    for (int i = 0; i < dx; i++) _object = _object.next;
                }

                /* priority sort direction */
                if ((object_list.sort_type & GFXOBJ_SORT_PRIORITY_BACK) != 0)
                {
                    start_priority = priorities - 1;
                    end_priority = -1;
                    dx = -1;
                }
                else
                {
                    start_priority = 0;
                    end_priority = priorities;
                    dx = 1;
                }
                /* link between priority */
                last_object = null;
                for (priority = start_priority; priority != end_priority; priority += dx)
                {
                    if (end_object[priority] != null)
                    {
                        if (last_object != null)
                            last_object.next = top_object[priority];
                        else
                            object_list.first_object = top_object[priority];
                        last_object = end_object[priority];
                    }
                }
                if (last_object == null)
                    object_list.first_object = null;
                else
                    last_object.next = null;
            }
            else
            {	/* non sort , update only linked object */
                for (_object = object_list.first_object; _object != null; _object = _object.next)
                {
                    /* update all objects */
                    if (_object.dirty_flag != 0)
                        object_update(_object);
                }
            }
            /* palette resource */
            if (_object.palette_flag != 0)
            {
                /* !!!!! do not supported yet !!!!! */
            }
#endif
        }

        public static void gfxobj_update()
        {
            gfx_object_list object_list;

            for (object_list = first_object_list; object_list != null; object_list = object_list.next)
                gfxobj_update_one(object_list);
        }
        static void draw_object_one(osd_bitmap bitmap, gfx_object _object)
        {
            throw new Exception();
        }
        public static void gfxobj_draw(gfx_object_list object_list)
        {
            osd_bitmap bitmap = Machine.scrbitmap;
            gfx_object _object;

            for (_object = object_list.first_object; _object != null; _object = _object.next)
            {
                if (_object.visible)
                    draw_object_one(bitmap, _object);
            }
        }

    }
}
