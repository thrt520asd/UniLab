using System;
using UnityEngine;


// Token: 0x02000730 RID: 1840
[Serializable]
public class SplitterState
{
    // Token: 0x06004159 RID: 16729 RVA: 0x0010EA36 File Offset: 0x0010CC36
    public SplitterState(params float[] relativeSizes)
    {
        this.Init(relativeSizes, null, null, 0);
    }

    // Token: 0x0600415A RID: 16730 RVA: 0x0010EA58 File Offset: 0x0010CC58
    public SplitterState(int[] realSizes, int[] minSizes, int[] maxSizes)
    {
        this.realSizes = realSizes;
        this.minSizes = ((minSizes != null) ? minSizes : new int[realSizes.Length]);
        this.maxSizes = ((maxSizes != null) ? maxSizes : new int[realSizes.Length]);
        this.relativeSizes = new float[realSizes.Length];
        this.splitSize = ((this.splitSize != 0) ? this.splitSize : 6);
        this.RealToRelativeSizes();
    }

    // Token: 0x0600415B RID: 16731 RVA: 0x0010EAE6 File Offset: 0x0010CCE6
    public SplitterState(float[] relativeSizes, int[] minSizes, int[] maxSizes)
    {
        this.Init(relativeSizes, minSizes, maxSizes, 0);
    }

    // Token: 0x0600415C RID: 16732 RVA: 0x0010EB07 File Offset: 0x0010CD07
    public SplitterState(float[] relativeSizes, int[] minSizes, int[] maxSizes, int splitSize)
    {
        this.Init(relativeSizes, minSizes, maxSizes, splitSize);
    }

    // Token: 0x0600415D RID: 16733 RVA: 0x0010EB2C File Offset: 0x0010CD2C
    private void Init(float[] relativeSizes, int[] minSizes, int[] maxSizes, int splitSize)
    {
        this.relativeSizes = relativeSizes;
        this.minSizes = ((minSizes != null) ? minSizes : new int[relativeSizes.Length]);
        this.maxSizes = ((maxSizes != null) ? maxSizes : new int[relativeSizes.Length]);
        this.realSizes = new int[relativeSizes.Length];
        this.splitSize = ((splitSize != 0) ? splitSize : 6);
        this.NormalizeRelativeSizes();
    }

    // Token: 0x0600415E RID: 16734 RVA: 0x0010EBA0 File Offset: 0x0010CDA0
    public void NormalizeRelativeSizes()
    {
        float num = 1f;
        float num2 = 0f;
        for (int i = 0; i < this.relativeSizes.Length; i++)
        {
            num2 += this.relativeSizes[i];
        }
        for (int i = 0; i < this.relativeSizes.Length; i++)
        {
            this.relativeSizes[i] = this.relativeSizes[i] / num2;
            num -= this.relativeSizes[i];
        }
        this.relativeSizes[this.relativeSizes.Length - 1] += num;
    }

    // Token: 0x0600415F RID: 16735 RVA: 0x0010EC30 File Offset: 0x0010CE30
    public void RealToRelativeSizes()
    {
        float num = 1f;
        float num2 = 0f;
        for (int i = 0; i < this.realSizes.Length; i++)
        {
            num2 += (float)this.realSizes[i];
        }
        for (int i = 0; i < this.realSizes.Length; i++)
        {
            this.relativeSizes[i] = (float)this.realSizes[i] / num2;
            num -= this.relativeSizes[i];
        }
        if (this.relativeSizes.Length > 0)
        {
            this.relativeSizes[this.relativeSizes.Length - 1] += num;
        }
    }

    // Token: 0x06004160 RID: 16736 RVA: 0x0010ECD0 File Offset: 0x0010CED0
    public void RelativeToRealSizes(int totalSpace)
    {
        int num = totalSpace;
        for (int i = 0; i < this.relativeSizes.Length; i++)
        {
            this.realSizes[i] = (int)Mathf.Round(this.relativeSizes[i] * (float)totalSpace);
            if (this.realSizes[i] < this.minSizes[i])
            {
                this.realSizes[i] = this.minSizes[i];
            }
            num -= this.realSizes[i];
        }
        if (num < 0)
        {
            for (int i = 0; i < this.relativeSizes.Length; i++)
            {
                if (this.realSizes[i] > this.minSizes[i])
                {
                    int num2 = this.realSizes[i] - this.minSizes[i];
                    int num3 = (-num >= num2) ? num2 : (-num);
                    num += num3;
                    this.realSizes[i] -= num3;
                    if (num >= 0)
                    {
                        break;
                    }
                }
            }
        }
        int num4 = this.realSizes.Length - 1;
        if (num4 >= 0)
        {
            this.realSizes[num4] += num;
            if (this.realSizes[num4] < this.minSizes[num4])
            {
                this.realSizes[num4] = this.minSizes[num4];
            }
        }
    }

    // Token: 0x06004161 RID: 16737 RVA: 0x0010EE14 File Offset: 0x0010D014
    public void DoSplitter(int i1, int i2, int diff)
    {
        int num = this.realSizes[i1];
        int num2 = this.realSizes[i2];
        int num3 = this.minSizes[i1];
        int num4 = this.minSizes[i2];
        int num5 = this.maxSizes[i1];
        int num6 = this.maxSizes[i2];
        bool flag = false;
        if (num3 == 0)
        {
            num3 = 16;
        }
        if (num4 == 0)
        {
            num4 = 16;
        }
        if (num + diff < num3)
        {
            diff -= num3 - num;
            this.realSizes[i2] += this.realSizes[i1] - num3;
            this.realSizes[i1] = num3;
            if (i1 != 0)
            {
                this.DoSplitter(i1 - 1, i2, diff);
            }
            else
            {
                this.splitterInitialOffset -= diff;
            }
            flag = true;
        }
        else if (num2 - diff < num4)
        {
            diff -= num2 - num4;
            this.realSizes[i1] += this.realSizes[i2] - num4;
            this.realSizes[i2] = num4;
            if (i2 != this.realSizes.Length - 1)
            {
                this.DoSplitter(i1, i2 + 1, diff);
            }
            else
            {
                this.splitterInitialOffset -= diff;
            }
            flag = true;
        }
        if (!flag)
        {
            if (num5 != 0 && num + diff > num5)
            {
                diff -= this.realSizes[i1] - num5;
                this.realSizes[i2] += this.realSizes[i1] - num5;
                this.realSizes[i1] = num5;
                if (i1 != 0)
                {
                    this.DoSplitter(i1 - 1, i2, diff);
                }
                else
                {
                    this.splitterInitialOffset -= diff;
                }
                flag = true;
            }
            else if (num6 != 0 && num2 - diff > num6)
            {
                diff -= num2 - num6;
                this.realSizes[i1] += this.realSizes[i2] - num6;
                this.realSizes[i2] = num6;
                if (i2 != this.realSizes.Length - 1)
                {
                    this.DoSplitter(i1, i2 + 1, diff);
                }
                else
                {
                    this.splitterInitialOffset -= diff;
                }
                flag = true;
            }
        }
        if (!flag)
        {
            this.realSizes[i1] += diff;
            this.realSizes[i2] -= diff;
        }
    }

    // Token: 0x04002094 RID: 8340
    private const int defaultSplitSize = 6;

    // Token: 0x04002095 RID: 8341
    public int ID;

    // Token: 0x04002096 RID: 8342
    public int splitterInitialOffset;

    // Token: 0x04002097 RID: 8343
    public int currentActiveSplitter = -1;

    // Token: 0x04002098 RID: 8344
    public int[] realSizes;

    // Token: 0x04002099 RID: 8345
    public float[] relativeSizes;

    // Token: 0x0400209A RID: 8346
    public int[] minSizes;

    // Token: 0x0400209B RID: 8347
    public int[] maxSizes;

    // Token: 0x0400209C RID: 8348
    public int lastTotalSize = 0;

    // Token: 0x0400209D RID: 8349
    public int splitSize;

    // Token: 0x0400209E RID: 8350
    public float xOffset;
}

