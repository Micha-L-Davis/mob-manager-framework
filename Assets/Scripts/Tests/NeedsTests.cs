
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

public class NeedsTests
{
    [Test]
    public void ProcessNeedsChangeTest()
    {

    }

    [Test]
    public void ResolveNeedsTest()
    {
        Mobile mobileNeedyObject = GameObject.FindObjectOfType<Mobile>();
        mobileNeedyObject._mobileSet.Items.Add(mobileNeedyObject);
        Stationary stationaryNeedyObject = GameObject.FindObjectOfType<Stationary>();
        stationaryNeedyObject._stationarySet.Items.Add(stationaryNeedyObject);
        
        Needs mobileNeeds = mobileNeedyObject.gameObject.GetComponent<Needs>();
        foreach (Need need in mobileNeeds._needsList.Items)
        {
            mobileNeeds.NeedsDictionary.Add(need, mobileNeeds._initialNeedValue);
        }
        mobileNeeds._needyObject = mobileNeedyObject;
        mobileNeeds._priorityNeed = null;
        
        Needs stationaryNeeds = stationaryNeedyObject.GetComponent<Needs>();
        foreach (Need need in stationaryNeeds._needsList.Items)
        {
            stationaryNeeds.NeedsDictionary.Add(need, stationaryNeeds._initialNeedValue);
        }
        stationaryNeeds._needyObject = stationaryNeedyObject;

        stationaryNeeds._priorityNeed = mobileNeedyObject._resolvableNeeds[0];
        
        stationaryNeeds.ResolveNeeds();

        Assert.That(stationaryNeeds._assignedResolver != null);
        Assert.That(mobileNeeds.NeedsDictionary.ContainsKey(stationaryNeedyObject._workerNeeds[0]));
        Assert.That(mobileNeeds._priorityNeed == stationaryNeedyObject._workerNeeds[0]);
        Assert.That(mobileNeeds._assignedResolver != null);
        Assert.That(mobileNeeds._assignedResolver == (INeedResolver)stationaryNeedyObject);
    }

    [Test]
    public void ResolverInRangeTest()
    {
        Mobile mobileNeedyObject = GameObject.FindObjectOfType<Mobile>();
        Stationary stationaryNeedyObject = GameObject.FindObjectOfType<Stationary>();

        Needs mobileNeeds = mobileNeedyObject.gameObject.GetComponent<Needs>();
        Needs stationaryNeeds = stationaryNeedyObject.gameObject.GetComponent<Needs>();

        mobileNeeds._assignedResolver = (INeedResolver)stationaryNeedyObject;
        stationaryNeeds._assignedResolver = (INeedResolver)mobileNeedyObject;

        mobileNeedyObject.gameObject.transform.position = new Vector3(10, 0, 0);
        stationaryNeedyObject.gameObject.transform.position = Vector3.zero;

        Assert.That(!mobileNeeds.ResolverInRange());
        Assert.That(!stationaryNeeds.ResolverInRange());

        mobileNeedyObject.gameObject.transform.position = new Vector3(0.25f, 0, 0);

        Assert.That(mobileNeeds.ResolverInRange());
        Assert.That(stationaryNeeds.ResolverInRange());
    }

    [Test]
    public void CalculatePrioritiesTest()
    {

    }

    [Test]
    public void FindAvailableResolverTest() 
    {
        Mobile mobileNeedyObject = GameObject.FindObjectOfType<Mobile>();
        mobileNeedyObject._mobileSet.Items.Add(mobileNeedyObject);
        Stationary stationaryNeedyObject = GameObject.FindObjectOfType<Stationary>();
        stationaryNeedyObject._stationarySet.Items.Add(stationaryNeedyObject);

        Needs mobileNeeds = mobileNeedyObject.gameObject.GetComponent<Needs>();
        Needs stationaryNeeds = stationaryNeedyObject.GetComponent<Needs>();

        mobileNeeds._priorityNeed = stationaryNeedyObject._resolvableNeeds[0];
        INeedResolver foundResolverForMobile = mobileNeeds.FindAvailableResolver(mobileNeeds._priorityNeed);

        Assert.That(foundResolverForMobile != null);
        Assert.That(foundResolverForMobile.Transform == stationaryNeedyObject.Transform);

        stationaryNeeds._priorityNeed = mobileNeedyObject._resolvableNeeds[0];
        INeedResolver foundResolverForStationary = stationaryNeeds.FindAvailableResolver(stationaryNeeds._priorityNeed);

        Assert.That(foundResolverForStationary != null);
        Assert.That(foundResolverForStationary.Transform == mobileNeedyObject.Transform);
    }

    [Test]
    public void AddNeedTest()
    {

    }

    [Test]
    public void ReplenishNeedTest()
    {

    }
}
