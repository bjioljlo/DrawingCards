using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CardGame;

namespace CardGame.UI
{
    public class DiscardPileForm : Form
    {
        private FlowLayoutPanel pnlDiscardPile;

        public DiscardPileForm(DiscardPile discardPile)
        {
            this.Text = "棄牌區";
            this.Size = new Size(500, 250);
            this.StartPosition = FormStartPosition.CenterParent;

            pnlDiscardPile = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(pnlDiscardPile);

            UpdateDiscardPile(discardPile);
        }

        public void UpdateDiscardPile(DiscardPile discardPile)
        {
            pnlDiscardPile.Controls.Clear();
            foreach (var card in discardPile)
            {
                var btn = new Button
                {
                    Text = card.Name,
                    Width = 60,
                    Height = 80,
                    Tag = card,
                    Enabled = false // 只顯示，不可操作
                };
                pnlDiscardPile.Controls.Add(btn);
            }
        }
    }
}
