/*! \cond PRIVATE */
using System;
using System.Collections.Generic;
using DarkTonic.PoolBoss;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(PoolBoss))]
// ReSharper disable once CheckNamespace
public class PoolBossInspector : Editor {
    private const string DoNotDestroyPoolItem = "This will destroy the Pool Item. Pool Items should only be despawned, never destroyed.";

    private PoolBoss _pool;
    private bool _isDirty;

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI() {
        EditorGUI.indentLevel = 0;

        _pool = (PoolBoss)target;

        _isDirty = false;

		var isInProjectView = DTPoolBossInspectorUtility.IsPrefabInProjectView(_pool);

        if (isInProjectView) {
			DTPoolBossInspectorUtility.ShowRedError("You have selected the PoolBoss prefab in Project View. Please select the one in your Scene to edit. Or, to create one in your Scene, drag this into the Hierarchy.");
            return;
        }

        var catNames = new List<string>(_pool._categories.Count);
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _pool._categories.Count; i++) {
            catNames.Add(_pool._categories[i].CatName);
        }

        if (!Application.isPlaying) {
			DTPoolBossInspectorUtility.StartGroupHeader();
            var newCat = EditorGUILayout.TextField("New Category Name", _pool.newCategoryName);
            if (newCat != _pool.newCategoryName) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change New Category Name");
                _pool.newCategoryName = newCat;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
			GUI.contentColor = DTPoolBossInspectorUtility.BrightButtonColor;
            GUILayout.Space(10);
            if (GUILayout.Button("Create New Category", EditorStyles.toolbarButton, GUILayout.Width(130))) {
                CreateCategory();
            }
            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            DTPoolBossInspectorUtility.AddSpaceForNonU5();
			DTPoolBossInspectorUtility.ResetColors();

            var selCatIndex = catNames.IndexOf(_pool.addToCategoryName);

            if (selCatIndex == -1) {
                selCatIndex = 0;
                _isDirty = true;
            }

            GUI.backgroundColor = DTPoolBossInspectorUtility.BrightButtonColor;

            var newIndex = EditorGUILayout.Popup("Default Item Category", selCatIndex, catNames.ToArray());
            if (newIndex != selCatIndex) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Default Item Category");
                _pool.addToCategoryName = catNames[newIndex];
            }
            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        GUI.contentColor = Color.white;

        if (!Application.isPlaying) {
            var maxFrames = Math.Min(100, _pool.poolItems.Count);
            maxFrames = Math.Max(1, maxFrames);
            var newFrames = EditorGUILayout.IntSlider(new GUIContent("Initialize Time (Frames)", "You can increase this value to make the initial pool creation take more frames. Defaults to 1. Max of the 100 or number of different prefabs, whichever is less."), _pool.framesForInit, 1, maxFrames);
            if (newFrames != _pool.framesForInit) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Initialize Time (Frames)");
                _pool.framesForInit = newFrames;
            }
        }

        PoolBossItem itemToRemove = null;
        int? indexToInsertAt = null;
        PoolBossCategory selectedCategory = null;
        PoolBossItem itemToClone = null;

        PoolBossCategory catEditing = null;
        PoolBossCategory catRenaming = null;

        PoolBossCategory catToDelete = null;
        int? indexToShiftUp = null;
        int? indexToShiftDown = null;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _pool.poolItems.Count; i++) {
            var item = _pool.poolItems[i];
            if (catNames.Contains(item.categoryName)) {
                continue;
            }

            item.categoryName = catNames[0];
            _isDirty = true;
        }

        var newAutoAdd = EditorGUILayout.Toggle(new GUIContent("Auto-Add Missing Items", "Auto-Add Missing Items to top Category"), _pool.autoAddMissingPoolItems);
        if (newAutoAdd != _pool.autoAddMissingPoolItems) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Auto-Add Missing Items");
            _pool.autoAddMissingPoolItems = newAutoAdd;
        }

        var newLog = EditorGUILayout.Toggle("Log Messages", _pool.logMessages);
        if (newLog != _pool.logMessages) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Log Messages");
            _pool.logMessages = newLog;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Actions", GUILayout.Width(100));
		GUI.contentColor = DTPoolBossInspectorUtility.BrightButtonColor;

        GUILayout.FlexibleSpace();

        var allExpanded = true;
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _pool._categories.Count; i++) {
            if (_pool._categories[i].IsExpanded) {
                continue;
            }
            allExpanded = false;
            break;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _pool.poolItems.Count; i++) {
            if (_pool.poolItems[i].isExpanded) {
                continue;
            }
            allExpanded = false;
            break;
        }

		if (Application.isPlaying) {
			if (GUILayout.Button(new GUIContent("Clear Peaks"), EditorStyles.toolbarButton, GUILayout.Width(80))) {
				ClearAllPeaks();
				_isDirty = true;
			}
			
			GUILayout.Space(6);
		}

        var buttonTooltip = allExpanded ? "Click to collapse all categories and items" : "Click to expand all categories and items";
        var buttonText = allExpanded ? "Collapse All" : "Expand All";
        if (GUILayout.Button(new GUIContent(buttonText, buttonTooltip), EditorStyles.toolbarButton, GUILayout.Width(80))) {
            ExpandCollapseAll(!allExpanded);
        }

        if (Application.isPlaying) {
            GUILayout.Space(6);
            if (GUILayout.Button(new GUIContent("Despawn All", "Click to despawn prefabs"), EditorStyles.toolbarButton, GUILayout.Width(80))) {
                PoolBoss.DespawnAllPrefabs();
                _isDirty = true;
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUI.backgroundColor = Color.white;

        if (!Application.isPlaying) {
            EditorGUILayout.BeginVertical();
            var anEvent = Event.current;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);
			GUI.color = DTPoolBossInspectorUtility.DragAreaColor;
            var dragArea = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true));
            GUI.Box(dragArea, "Drag prefabs here in bulk to add them to the Pool!");
            GUI.color = Color.white;

            switch (anEvent.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragArea.Contains(anEvent.mousePosition)) {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (anEvent.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences) {
                            AddPoolItem(dragged);
                        }
                    }
                    Event.current.Use();
                    break;
            }
            GUILayout.Space(4);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

			DTPoolBossInspectorUtility.VerticalSpace(4);
        }

        GUI.backgroundColor = Color.white;
        GUI.contentColor = Color.white;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var c = 0; c < _pool._categories.Count; c++) {
            var cat = _pool._categories[c];

            EditorGUI.indentLevel = 0;

            var matchingItems = new List<PoolBossItem>();
            matchingItems.AddRange(_pool.poolItems);
            matchingItems.RemoveAll(delegate (PoolBossItem x) {
                return x.categoryName != cat.CatName;
            });

            var hasItems = matchingItems.Count > 0;

            EditorGUILayout.BeginHorizontal();

            if (!cat.IsEditing || Application.isPlaying) {
                var catName = cat.CatName;

                if (!cat.IsExpanded) {
                    catName += ": " + matchingItems.Count + " item" + ((matchingItems.Count != 1) ? "s" : "");
                }

                var state = cat.IsExpanded;
                var text = catName;

                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (!state) {
                    GUI.backgroundColor = DTPoolBossInspectorUtility.InactiveHeaderColor;
                } else {
					GUI.backgroundColor = DTPoolBossInspectorUtility.ActiveHeaderColor;
                }

                text = "<b><size=11>" + text + "</size></b>";

                if (state) {
                    text = "\u25BC " + text;
                } else {
                    text = "\u25BA " + text;
                }
                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) {
                    state = !state;
                }

                GUILayout.Space(2f);

                if (state != cat.IsExpanded) {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle expand Pool Boss Category");
                    cat.IsExpanded = state;
                }

                var catItemsCollapsed = true;

                for (var i = 0; i < _pool.poolItems.Count; i++) {
                    var item = _pool.poolItems[i];
                    if (item.categoryName != cat.CatName) {
                        continue;
                    }

                    if (!item.isExpanded) {
                        continue;
                    }
                    catItemsCollapsed = false;
                    break;
                }

                GUI.backgroundColor = Color.white;

                var tooltip = catItemsCollapsed ? "Click to expand all items in this category" : "Click to collapse all items in this category";
                var btnText = catItemsCollapsed ? "Expand" : "Collapse";

				GUI.contentColor = DTPoolBossInspectorUtility.BrightButtonColor;
                if (GUILayout.Button(new GUIContent(btnText, tooltip), EditorStyles.toolbarButton, GUILayout.Width(60), GUILayout.Height(16))) {
                    ExpandCollapseCategory(cat.CatName, catItemsCollapsed);
                }
                GUI.contentColor = Color.white;

                if (!Application.isPlaying) {
                    if (c > 0) {
                        // the up arrow.
						var upArrow = PoolBossInspectorResources.UpArrowTexture;
                        if (GUILayout.Button(new GUIContent(upArrow, "Click to shift Category up"),
                            EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16))) {
                            indexToShiftUp = c;
                        }
                    } else {
                        GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16));
                    }

                    if (c < _pool._categories.Count - 1) {
                        // The down arrow will move things towards the end of the List
						var dnArrow = PoolBossInspectorResources.DownArrowTexture;
                        if (GUILayout.Button(new GUIContent(dnArrow, "Click to shift Category down"),
                            EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16))) {
                            indexToShiftDown = c;
                        }
                    } else {
                        GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16));
                    }

                    var settingsIcon = new GUIContent(PoolBossInspectorResources.SettingsTexture,
                        "Click to edit Category");

                    GUI.backgroundColor = Color.white;
                    if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton, GUILayout.Width(24),
                        GUILayout.Height(16))) {
                        catEditing = cat;
                    }
                    GUI.backgroundColor = DTPoolBossInspectorUtility.DeleteButtonColor;
                    if (GUILayout.Button(new GUIContent("Delete", "Click to delete Category"),
                        EditorStyles.miniButton, GUILayout.MaxWidth(45))) {
                        catToDelete = cat;
                    }
                    GUILayout.Space(15);
                } else {
                    GUI.contentColor = DTPoolBossInspectorUtility.BrightButtonColor;

                    if (GUILayout.Button(new GUIContent("Despawn All", "Click to despawn all prefabs in this Category"), EditorStyles.toolbarButton, GUILayout.Width(80))) {
                        PoolBoss.DespawnAllPrefabsInCategory(cat.CatName);
                        _isDirty = true;
                    }

                    var itemsSpawned = PoolBoss.CategoryItemsSpawned(cat.CatName);
                    var categoryHasItemsSpawned = itemsSpawned > 0;
                    var theBtnText = itemsSpawned.ToString();
					var btnColor = categoryHasItemsSpawned ? DTPoolBossInspectorUtility.BrightTextColor : DTPoolBossInspectorUtility.DeleteButtonColor;
                    GUI.backgroundColor = btnColor;

                    var btnWidth = 32;
                    if (theBtnText.Length > 3) {
                        btnWidth = 11 * theBtnText.Length;
                    }
                    GUILayout.Button(theBtnText, EditorStyles.miniButtonRight, GUILayout.MaxWidth(btnWidth));

                    GUI.backgroundColor = Color.white;
                    GUI.contentColor = Color.white;
                    GUILayout.Space(4);
                }
            } else {
                GUI.backgroundColor = DTPoolBossInspectorUtility.BrightTextColor;
                var tex = EditorGUILayout.TextField("", cat.ProspectiveName);
                if (tex != cat.ProspectiveName) {
                    cat.ProspectiveName = tex;
                    _isDirty = true;
                }

				var buttonPressed = DTPoolBossInspectorUtility.AddCancelSaveButtons("category");

                switch (buttonPressed) {
                    case DTPoolBossInspectorUtility.FunctionButtons.Cancel:
                        cat.IsEditing = false;
                        cat.ProspectiveName = cat.CatName;
                        _isDirty = true;
                        break;
					case DTPoolBossInspectorUtility.FunctionButtons.Save:
                        catRenaming = cat;
						_isDirty = true;
                        break;
                }

                GUILayout.Space(15);
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            if (cat.IsEditing) {
				DTPoolBossInspectorUtility.VerticalSpace(2);
            }

            matchingItems.Sort(delegate (PoolBossItem x, PoolBossItem y) {
                if (x.prefabTransform == null && y.prefabTransform != null) {
                    return -1;
                }
                if (y.prefabTransform == null && x.prefabTransform != null) {
                    return 1;
                }
                if (y.prefabTransform == null && x.prefabTransform == null) {
                    return 0;
                }

                // ReSharper disable PossibleNullReferenceException
                return String.Compare(x.prefabTransform.name, y.prefabTransform.name, StringComparison.Ordinal);
                // ReSharper restore PossibleNullReferenceException
            });

            if (!hasItems) {
				DTPoolBossInspectorUtility.BeginGroupedControls();
				DTPoolBossInspectorUtility.ShowLargeBarAlert("This Category is empty. Add / move some items or you may delete it.");
				DTPoolBossInspectorUtility.EndGroupedControls();
            }

            if (cat.IsExpanded) {
                if (matchingItems.Count > 0) {
					DTPoolBossInspectorUtility.BeginGroupedControls();
                }

                for (var i = 0; i < matchingItems.Count; i++) {
                    var poolItem = matchingItems[i];

					DTPoolBossInspectorUtility.StartGroupHeader();

                    if (poolItem.prefabTransform != null) {
                        var rend = poolItem.prefabTransform.GetComponent<TrailRenderer>();
                        if (rend != null && rend.autodestruct) {
							DTPoolBossInspectorUtility.ShowRedError(
                                "This prefab contains a Trail Renderer with auto-destruct enabled. " + DoNotDestroyPoolItem);
                        } else {
#if UNITY_5_4_OR_NEWER
    // nothing to check
#else
                            var partAnim = poolItem.prefabTransform.GetComponent<ParticleAnimator>();
                            if (partAnim != null && partAnim.autodestruct) {
								DTPoolBossInspectorUtility.ShowRedError(
                                    "This prefab contains a Particle Animator with auto-destruct enabled. " + DoNotDestroyPoolItem);
                            }
#endif
                        }
                    }

                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.BeginHorizontal();
                    var itemName = poolItem.prefabTransform == null ? "[NO PREFAB]" : poolItem.prefabTransform.name;
					var state = DTPoolBossInspectorUtility.Foldout(poolItem.isExpanded, itemName);
                    if (state != poolItem.isExpanded) {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle expand Pool Item");
                        poolItem.isExpanded = state;
                    }

                    if (Application.isPlaying) {
                        GUILayout.FlexibleSpace();

						GUI.contentColor = DTPoolBossInspectorUtility.BrightTextColor;
                        if (poolItem.prefabTransform != null) {
                            if (GUILayout.Button(new GUIContent("Despawn All", "Click to despawn all of this prefab"),
                                EditorStyles.toolbarButton, GUILayout.Width(80))) {
                                PoolBoss.DespawnAllOfPrefab(poolItem.prefabTransform);
                                _isDirty = true;
                            }

                            var itemInfo = PoolBoss.PoolItemInfoByName(itemName);
                            if (itemInfo != null) {
                                var spawnedCount = itemInfo.SpawnedClones.Count;
                                var despawnedCount = itemInfo.DespawnedClones.Count;
                                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                                if (spawnedCount == 0) {
									GUI.backgroundColor = DTPoolBossInspectorUtility.DeleteButtonColor;
                                } else {
									GUI.backgroundColor = DTPoolBossInspectorUtility.BrightTextColor;
                                }
                                var content =
                                    new GUIContent(
                                        string.Format("{0} / {1} Spawned", spawnedCount,
                                            despawnedCount + spawnedCount),
                                        "Click here to select all spawned items.");
                                if (GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Width(110))) {
                                    var obj = new List<GameObject>();
                                    foreach (var t in itemInfo.SpawnedClones) {
                                        obj.Add(t.gameObject);
                                    }

                                    if (obj.Count > 0) {
                                        Selection.objects = obj.ToArray();
                                    }
                                }

								content = new GUIContent("Pk: " + itemInfo.Peak, "Click to reset peak to zero.");
								if (Time.realtimeSinceStartup - itemInfo.PeakTime < .2f) {
									GUI.backgroundColor = DTPoolBossInspectorUtility.AddButtonColor;
								} else if (itemInfo.Peak == 0) {
									GUI.backgroundColor = DTPoolBossInspectorUtility.DeleteButtonColor;
								} else {
									GUI.backgroundColor = DTPoolBossInspectorUtility.BrightTextColor;
								}
								
								if (GUILayout.Button(content, EditorStyles.miniButton, GUILayout.Width(64))) {
									itemInfo.Peak = Math.Max(0, itemInfo.SpawnedClones.Count);
									itemInfo.PeakTime = Time.realtimeSinceStartup;
									_isDirty = true;
									_pool._changes++;
								}
								GUI.backgroundColor = Color.white;
							}
                        }
                        GUI.contentColor = Color.white;
                    } else {
						GUI.backgroundColor = DTPoolBossInspectorUtility.BrightButtonColor;
                        var selCatIndex = catNames.IndexOf(poolItem.categoryName);
                        var newCat = EditorGUILayout.Popup(selCatIndex, catNames.ToArray(), GUILayout.Width(130));
                        if (newCat != selCatIndex) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Pool Item Category");
                            poolItem.categoryName = catNames[newCat];
                        }
                        GUI.backgroundColor = Color.white;

						DTPoolBossInspectorUtility.FocusInProjectViewButton("Pool Item prefab", poolItem.prefabTransform == null ? null : poolItem.prefabTransform.gameObject);
                    }

                    var buttonPressed = DTPoolBossInspectorUtility.AddFoldOutListItemButtons(i, matchingItems.Count,
                        "Pool Item", false, null, true, false, true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    if (poolItem.isExpanded) {
                        EditorGUI.indentLevel = 0;

                        var newPrefab =
                            (Transform)
                                EditorGUILayout.ObjectField("Prefab", poolItem.prefabTransform, typeof(Transform),
                                    false);
                        if (newPrefab != poolItem.prefabTransform) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Pool Item Prefab");
                            poolItem.prefabTransform = newPrefab;
                        }

                        var newPreloadQty = EditorGUILayout.IntSlider("Preload Qty", poolItem.instancesToPreload, 0,
                            10000);
                        if (newPreloadQty != poolItem.instancesToPreload) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool,
                                "change Pool Item Preload Qty");
                            poolItem.instancesToPreload = newPreloadQty;
                        }
                        if (poolItem.instancesToPreload == 0) {
                            DTPoolBossInspectorUtility.ShowColorWarning(
                                "You have set the Preload Qty to 0. This prefab will not be in the Pool.");
                        }

                        var newAllow = EditorGUILayout.Toggle("Allow Instantiate More",
                            poolItem.allowInstantiateMore);
                        if (newAllow != poolItem.allowInstantiateMore) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool,
                                "toggle Allow Instantiate More");
                            poolItem.allowInstantiateMore = newAllow;
                        }

                        if (poolItem.allowInstantiateMore) {
                            var newLimit = EditorGUILayout.IntSlider("Item Limit", poolItem.itemHardLimit,
                                 poolItem.instancesToPreload, 10000);
                            if (newLimit != poolItem.itemHardLimit) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Item Limit");
                                poolItem.itemHardLimit = newLimit;
                            }
                        } else {
                            var newRecycle = EditorGUILayout.Toggle("Recycle Oldest", poolItem.allowRecycle);
                            if (newRecycle != poolItem.allowRecycle) {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Recycle Oldest");
                                poolItem.allowRecycle = newRecycle;
                            }
                        }

                        newLog = EditorGUILayout.Toggle("Log Messages", poolItem.logMessages);
                        if (newLog != poolItem.logMessages) {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle Log Messages");
                            poolItem.logMessages = newLog;
                        }
                    }

                    switch (buttonPressed) {
						case DTPoolBossInspectorUtility.FunctionButtons.Remove:
                            itemToRemove = poolItem;
                            break;
						case DTPoolBossInspectorUtility.FunctionButtons.Add:
                            indexToInsertAt = _pool.poolItems.IndexOf(poolItem);
                            selectedCategory = cat;
                            break;
						case DTPoolBossInspectorUtility.FunctionButtons.DespawnAll:
                            PoolBoss.DespawnAllOfPrefab(poolItem.prefabTransform);
                            break;
						case DTPoolBossInspectorUtility.FunctionButtons.Copy:
                            itemToClone = poolItem;
                            break;
                    }

                    EditorGUILayout.EndVertical();
					DTPoolBossInspectorUtility.AddSpaceForNonU5();
                }

                if (matchingItems.Count > 0) {
					DTPoolBossInspectorUtility.EndGroupedControls();
					DTPoolBossInspectorUtility.VerticalSpace(2);
                }
            }

			DTPoolBossInspectorUtility.VerticalSpace(2);
        }

        if (indexToShiftUp.HasValue) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "shift up Category");
            var item = _pool._categories[indexToShiftUp.Value];
            _pool._categories.Insert(indexToShiftUp.Value - 1, item);
            _pool._categories.RemoveAt(indexToShiftUp.Value + 1);
            _isDirty = true;
        }

        if (indexToShiftDown.HasValue) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "shift down Category");
            var index = indexToShiftDown.Value + 1;
            var item = _pool._categories[index];
            _pool._categories.Insert(index - 1, item);
            _pool._categories.RemoveAt(index + 1);
            _isDirty = true;
        }

        if (catToDelete != null) {
            if (_pool.poolItems.FindAll(delegate (PoolBossItem x) {
                return x.categoryName == catToDelete.CatName;
            }).Count > 0) {
				DTPoolBossInspectorUtility.ShowAlert("You cannot delete a Category with Pool Items in it. Move or delete the items first.");
            } else if (_pool._categories.Count <= 1) {
				DTPoolBossInspectorUtility.ShowAlert("You cannot delete the last Category.");
            } else {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "Delete Category");
                _pool._categories.Remove(catToDelete);
                _isDirty = true;
            }
        }

        if (catRenaming != null) {
            // ReSharper disable once ForCanBeConvertedToForeach
            var isValidName = true;

            if (string.IsNullOrEmpty(catRenaming.ProspectiveName)) {
                isValidName = false;
				DTPoolBossInspectorUtility.ShowAlert("You cannot have a blank Category name.");
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var c = 0; c < _pool._categories.Count; c++) {
                var cat = _pool._categories[c];
                // ReSharper disable once InvertIf
                if (cat != catRenaming && cat.CatName == catRenaming.ProspectiveName) {
                    isValidName = false;
					DTPoolBossInspectorUtility.ShowAlert("You already have a Category named '" + catRenaming.ProspectiveName + "'. Category names must be unique.");
                }
            }

            if (isValidName) {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "Undo change Category name.");

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _pool.poolItems.Count; i++) {
                    var item = _pool.poolItems[i];
                    if (item.categoryName == catRenaming.CatName) {
                        item.categoryName = catRenaming.ProspectiveName;
                    }
                }

                catRenaming.CatName = catRenaming.ProspectiveName;
                catRenaming.IsEditing = false;
                _isDirty = true;
            }
        }

        if (catEditing != null) {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var c = 0; c < _pool._categories.Count; c++) {
                var cat = _pool._categories[c];
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (catEditing == cat) {
                    cat.IsEditing = true;
                } else {
                    cat.IsEditing = false;
                }

                _isDirty = true;
            }
        }

        if (itemToRemove != null) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "remove Pool Item");
            _pool.poolItems.Remove(itemToRemove);
        }
        if (itemToClone != null) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "clone Pool Item");
            var newItem = itemToClone.Clone();

            var oldIndex = _pool.poolItems.IndexOf(itemToClone);

            _pool.poolItems.Insert(oldIndex, newItem);
        }

        if (indexToInsertAt.HasValue) {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "insert Pool Item");
            _pool.poolItems.Insert(indexToInsertAt.Value, new PoolBossItem {
                categoryName = selectedCategory.CatName
            });
        }

        if (GUI.changed || _isDirty) {
            EditorUtility.SetDirty(target);	// or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    private void ExpandCollapseAll(bool isExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle expand / collapse all Pool Boss Items");

        foreach (var cat in _pool._categories) {
            cat.IsExpanded = isExpand;
        }

        foreach (var item in _pool.poolItems) {
            item.isExpanded = isExpand;
        }
    }

    private void ExpandCollapseCategory(string category, bool isExpand) {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "toggle expand / collapse all items in Category");

        foreach (var cat in _pool._categories) {
            if (cat.CatName != category) {
                continue;
            }
            cat.IsExpanded = isExpand;
        }

        foreach (var item in _pool.poolItems) {
            if (item.categoryName != category) {
                continue;
            }

            item.isExpanded = isExpand;
        }
    }

    private void AddPoolItem(Object o) {
        // ReSharper disable once PossibleNullReferenceException
        var go = (o as GameObject);
        if (go == null) {
			DTPoolBossInspectorUtility.ShowAlert("You dragged an object which was not a Game Object. Not adding to Pool Boss.");
            return;
        }

        var newItem = new PoolBossItem {
            prefabTransform = go.transform,
            categoryName = _pool.addToCategoryName
        };

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "add Pool Boss Item");

        _pool.poolItems.Add(newItem);
    }

	private void ClearAllPeaks() {
		for (var i = 0; i < _pool.poolItems.Count; i++) {
			var poolItem = _pool.poolItems[i].prefabTransform;
			if (poolItem == null) {
				continue;
			}
			
			var item = PoolBoss.PoolItemInfoByName(poolItem.name);
			item.Peak = Math.Max(0, item.SpawnedClones.Count);
			item.PeakTime = Time.realtimeSinceStartup;
		}
		
		_isDirty = true;
		_pool._changes++;
	}

    private void CreateCategory() {
        if (string.IsNullOrEmpty(_pool.newCategoryName)) {
			DTPoolBossInspectorUtility.ShowAlert("You cannot have a blank Category name.");
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var c = 0; c < _pool._categories.Count; c++) {
            var cat = _pool._categories[c];
            // ReSharper disable once InvertIf
            if (cat.CatName == _pool.newCategoryName) {
				DTPoolBossInspectorUtility.ShowAlert("You already have a Category named '" + _pool.newCategoryName + "'. Category names must be unique.");
                return;
            }
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "Create New Category");

        var newCat = new PoolBossCategory {
            CatName = _pool.newCategoryName,
            ProspectiveName = _pool.newCategoryName,
        };

        _pool._categories.Add(newCat);
    }
}
/*! \endcond */