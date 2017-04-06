function X Y

 Moves rect of this. X Y are device coord.

OffsetRect +&m_rd X Y
memcpy &x &m_rd 16; DPtoLP(m_hdc +&x 2)
POINT p; GetViewportOrgEx m_hdc &p; OffsetRect +&x p.x p.y
